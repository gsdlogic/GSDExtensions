Param
(
    [Hashtable]
    $Variables = @{}
)

function Add-DateTime
{
    Param
    (
        [Parameter(ValueFromPipeline)]
        $object
    )

    $result = $object.Clone()
    $now = [DateTime]::UtcNow

    $result["Date"] = $now
	$result["DayOfYear"] = $now.DayOfYear
	$result["SecondsOfDay"] = ([Int32]($now.TimeOfDay.TotalSeconds))

    Write-Output $result
}

function Add-GitVariables
{
    Param
    (
        [Parameter(ValueFromPipeline)]
        $object
    )

    $result = $object.Clone()

    Write-Host ""
    Write-Host "$(git version 2>&1)"

    $result["Branch"] = (git rev-parse --abbrev-ref HEAD 2>&1)
    $result["CommitHash"] = (git rev-parse --short HEAD 2>&1)
    $result["CommitCount"] = ([Int32]::Parse((git rev-list --count HEAD 2>&1)))

    $commitDate = [DateTime]::Parse((git log -1 --format=%cd --date=iso 2>&1)).ToUniversalTime()

    $result["CommitDate"] = $commitDate
	$result["CommitDayOfYear"] = $commitDate.DayOfYear
	$result["CommitSecondsOfDay"] = ([Int32]($commitDate.TimeOfDay.TotalSeconds))

    $tag = (git tag 2>&1)

    if (-not [String]::IsNullOrEmpty($tag))
    {
        $tag = @(git describe 2>&1)[-1]
        $match = [Text.RegularExpressions.Regex]::Match($tag, "v(\d+(?:\.\d+){0,4})(?:-([a-zA-Z]+))?(?:-(\d+))?(?:-g([\da-f]{0,7}))?")

        $result["Tag"] = $tag
        $result["TagVersion"] = $match.Captures.Groups[1].Value
        $result["TagReleaseType"] = $match.Captures.Groups[2].Value
        $result["TagCommitCount"] = $match.Captures.Groups[3].Value
        $result["TagCommitHash"] = $match.Captures.Groups[4].Value
    }

    Write-Output $result
}

function ConvertTo-Hashtable
{
    Param
    (
        [Parameter(ValueFromPipeline)]
        $object
    )

    $properties = Get-Member -InputObject $object -MemberType NoteProperty
    $result = @{}

    foreach ($property in $properties)
    {
        $value = $object | Select-Object -ExpandProperty $property.Name
        $result[$property.Name] = $value
    }

    Write-Output $result
}

function Expand-Files
{
    Param
    (
        $files,
        $variables
    )

    Write-Host ""

    foreach ($file in $files)
    {
        foreach ($spec in $file.FileSpecs)
        {
            foreach ($info in (Get-ChildItem $spec -Recurse))
            {
                $content = [IO.File]::ReadAllText($info.FullName)
                $original = $content

                foreach ($pattern in $file.Patterns)
                {
                    $search = Expand-Value $pattern.Search $variables
                    $replace = Expand-Value $pattern.Replace $variables
                    $content = $content -creplace $search, $replace
                }

                if ($content -ne $original)
                {
                    Write-Host "Updating $($info.FullName)..."
                    Set-ItemProperty $info.FullName IsReadOnly $false
                    [IO.File]::WriteAllText($info.FullName, $content)
                }
            }
        }
    }
}

function Expand-Value
{
    Param
    (
        [Parameter(ValueFromPipeline)]
        $value,
        $variables
    )

    foreach ($field in $variables.GetEnumerator())
    {
        $pattern = "\{($($field.Name))(,[^:\}]*){0,1}(:[^\}]*){0,1}\}"
        $match = [System.Text.RegularExpressions.Regex]::Match($value, $pattern)

        if ($match.Success)
        {
            $group2 = $match.Groups[2].Value
            $group3 = $match.Groups[3].Value
            $format = "{0$group2$group3}"
            $fieldValue = $field.Value

            $type = if ($fieldValue -eq $null) { "Null" } else { $fieldValue.GetType().Name }

            # String format specifiers
            #
            # "L", "l"  Lower case
            # "U", "u"  Upper case
            #
            # "("       Surround with parenthesis
            # ")"       Surround with parenthesis if not empty
            #
            # "-X"      Prefix a dash
            # "-x"      Prefix a dash if not empty
            # "X-"      Postfix a dash
            # "x-"      Postfix a dash if not empty
            #
            # "_X"      Prefix a dash
            # "_x"      Prefix a dash if not empty
            # "X_"      Postfix a dash
            # "x_"      Postfix a dash if not empty

            if ($type -eq "String")
            {
                if ($group3.ToUpperInvariant().Contains("U"))
                {
                    $fieldValue = $fieldValue.ToUpperInvariant()
                }
                elseif ($group3.ToUpperInvariant().Contains("L"))
                {
                    $fieldValue = $fieldValue.ToLowerInvariant()
                }

                if ($group3.Contains("("))
                {
                    $fieldValue = "($fieldValue)"
                }
                elseif ($group3.Contains(")") -and -not [String]::IsNullOrEmpty($fieldValue))
                {
                    $fieldValue = "($fieldValue)"
                }
                
                if ($group3.Contains("-X"))
                {
                    $fieldValue = "-$fieldValue"
                }
                elseif ($group3.Contains("-x") -and -not [String]::IsNullOrEmpty($fieldValue))
                {
                    $fieldValue = "-$fieldValue"
                }
                elseif ($group3.Contains("X-"))
                {
                    $fieldValue = "$fieldValue-"
                }
                elseif ($group3.Contains("x-") -and -not [String]::IsNullOrEmpty($fieldValue))
                {
                    $fieldValue = "$fieldValue-"
                }
                
                if ($group3.Contains("_X"))
                {
                    $fieldValue = " $fieldValue"
                }
                elseif ($group3.Contains("_x") -and -not [String]::IsNullOrEmpty($fieldValue))
                {
                    $fieldValue = " $fieldValue"
                }
                elseif ($group3.Contains("X_"))
                {
                    $fieldValue = "$fieldValue "
                }
                elseif ($group3.Contains("x_") -and -not [String]::IsNullOrEmpty($fieldValue))
                {
                    $fieldValue = "$fieldValue "
                }
            }

            try
            {
                $formatted = [String]::Format($format, $fieldValue)
            }
            catch
            {
                Write-Warning "Invalid format: $($variable.Value)"
            }

            $value = [System.Text.RegularExpressions.Regex]::Replace($value, $pattern, $formatted)
        }
    }

    Write-Output $value
}

function Expand-Variables
{
    Param
    (
        [Parameter(ValueFromPipeline)]
        $object
    )

    $result = $object.Clone()

    foreach ($variable in $object.GetEnumerator())
    {
        $type = if ($variable.Value -eq $null) { "Null" } else { $variable.Value.GetType().Name }

        if ($type -ne "String")
        {
            continue
        }

        $result[$variable.Name] = Expand-Value $variable.Value $object
    }

    Write-Output $result
}

function Merge-Variables
{
    Param
    (
        [Parameter(ValueFromPipeline)]
        $object,
        $with
    )

    $result = $object.Clone()

    foreach ($variable in $with.GetEnumerator())
    {
        $result[$variable.Name] = $variable.Value
    }

    Write-Output $result
}

function Set-ReleaseType
{
    Param
    (
        [Parameter(ValueFromPipeline)]
        $object
    )

    $result = $object.Clone()

    if ([String]::IsNullOrEmpty($object["ReleaseType"]) -or $object["ReleaseType"].ToUpperInvariant() -eq "RELEASE")
    {
        $result["ReleaseType"] = ""
    }

    Write-Output $result
}

function Set-Version
{
    Param
    (
        [Parameter(ValueFromPipeline)]
        $object
    )

    $result = $object.Clone()

    if ([String]::IsNullOrEmpty($object["ProductVersion"]))
    {
        if ([String]::IsNullOrEmpty($object["TagVersion"]))
        {
            $result["ProductVersion"] = "0.0.0"
        }
        elseif ($object["TagCommitCount"] -gt 0)
        {
            $version = [Version]::Parse($object["TagVersion"])

            if ($version.Revision -ge 0)
            {
                $result["ProductVersion"] = "$($version.Major).$($version.Minor).$($version.Build).$($version.Revision + 1)"
            }
            elseif ($version.Build -ge 0)
            {
                $result["ProductVersion"] = "$($version.Major).$($version.Minor).$($version.Build + 1)"
            }
            elseif ($version.Minor -ge 0)
            {
                $result["ProductVersion"] = "$($version.Major).$($version.Minor + 1)"
            }
            else
            {
                $result["ProductVersion"] = "$($version.Major + 1)"
            }
        }
        else
        {
            $result["ProductVersion"] = $object["TagVersion"]
        }
    }

    $result["Version"] = ([Version]::Parse($result["ProductVersion"]))
    $result["Major"] = $result.Version.Major
    $result["Minor"] = $result.Version.Minor
    $result["Build"] = $result.Version.Build
    $result["Revision"] = $result.Version.Revision
    $result["Patch"] = $result.Version.Build

    Write-Output $result
}

function Write-Variables
{
    Param
    (
        [Parameter(ValueFromPipeline)]
        $object
    )

    Write-Host ""

    foreach ($variable in $object.GetEnumerator() | Sort-Object -Property Name)
    {
        $type = if ($variable.Value -eq $null) { "Null" } else { $variable.Value.GetType().Name }
        Write-Host ([String]::Format("{0,-28} {1,-10} {2}", $variable.Name, $type, $variable.Value))
    }
}

function Write-VSO
{
    Param
    (
        [Parameter(ValueFromPipeline)]
        $object
    )

    foreach ($variable in $object.GetEnumerator() | Sort-Object -Property Name)
    {
        Write-Host "##vso[task.setvariable variable=VER_$($variable.Name);]$($variable.Value)"
    }
}

$config = [IO.File]::ReadAllText([IO.Path]::Combine("$PSScriptRoot", "SetVersion.json")) | ConvertFrom-Json

Write-Host "SetVersion PowerShell Script Version 1.0"
Write-Host "Configuration Version $($config.Version)"

$variables = $config.Variables |
    ConvertTo-Hashtable |
    Add-DateTime |
    Add-GitVariables |
    Merge-Variables -with $Variables |
    Set-Version |
    Set-ReleaseType |
    Expand-Variables

Write-Variables $variables
Write-VSO $variables

Expand-Files $config.Files $variables
