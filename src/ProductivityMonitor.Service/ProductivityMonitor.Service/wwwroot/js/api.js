
function getPageDomain() {
    let hostName = `${location.protocol}//${window.location.hostname}`;
    if (window.location.port !== 80)
        hostName += `:${window.location.port}`;
    return hostName;
}

function getApiUrl() {
    return `${getPageDomain()}/api`;
}

async function getApplicationProductivity(startDate, endDate) {

    const urlString = _generateUrlString("data/applications/productivity", startDate, endDate);

    return await fetch(urlString,
        {
            method: "get"
        }).then(response => {
        return response.json();
    });
};

async function getKeyPresses(startDate, endDate) {
    const urlString = _generateUrlString("data/KeyPresses", startDate, endDate);

    return await fetch(urlString,
        {
            method: "Get"
        }).then(response => {
        return response.json();
    });
}

async function getKeyPressSummary(startDate, endDate) {
    const urlString = _generateUrlString("data/KeyPresses/Summary", startDate, endDate);

    return await fetch(urlString,
        {
            method: "Get"
        }).then(response => {
        return response.json();
    });
}

async function GetMouseMovements(startDate, endDate) {
    const urlString = _generateUrlString("data/mouse/movements", startDate, endDate);

    return await fetch(urlString,
        {
            method: "Get"
        }).then(response => {
        return response.json();
    });
}

async function GetTotalMouseDistance(startDate, endDate) {

    const urlString = _generateUrlString("data/mouse/movements/totalDistance", startDate, endDate);

    return await fetch(urlString,
        {
            method: "Get"
        }).then(response => {
        return response.json();
    });
}

async function getMouseMovements(startDate, endDate) {
    const urlString = _generateUrlString("data/mouse/clicks", startDate, endDate);

    return await fetch(urlString,
        {
            method: "Get"
        }).then(response => {
        return response.json();
    });
}

async function getMouseMovementsSummary(startDate, endDate, quadrantSizePx = 100) {

    const urlString = `${getApiUrl()}/data/mouse/clicks/summary`;
    const url = new URL(urlString);
    const parameters = new URLSearchParams(url);

    if (startDate != null)
        parameters.append("startDate", startDate);
    if (endDate != null)
        parameters.append("endDate", endDate);
    parameters.append("quadrantSizePx", quadrantSizePx);

    return await fetch(urlString,
        {
            method: "Get"
        }).then(response => {
        return response.json();
    });
}

function _generateUrlString(urlPath, startDate, endDate) {

    const urlString = `${getApiUrl()}/${urlPath}`;
    const url = new URL(urlString);
    const parameters = new URLSearchParams(url);

    if (startDate != null)
        parameters.append("startDate", startDate);
    if (endDate != null)
        parameters.append("endDate", endDate);

    url.search = parameters.toString();

    const outputUrlString = url.toString();
    return outputUrlString;
}