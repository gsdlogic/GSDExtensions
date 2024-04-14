export function getTimezoneOffset() {
    return new Date().getTimezoneOffset();
}

export function getTimezoneName() {
    return Intl.DateTimeFormat().resolvedOptions().timeZone;
}
