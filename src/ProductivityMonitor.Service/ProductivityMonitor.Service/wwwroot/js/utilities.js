const internalColors = {
    red: 'rgb(255, 99, 132)',
    orange: 'rgb(255, 159, 64)',
    yellow: 'rgb(255, 205, 86)',
    green: 'rgb(75, 192, 192)',
    blue: 'rgb(54, 162, 235)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(201, 203, 207)'
}

function mapApplicationDataForGraph(records) {

    const chartData = {
        labels: [],
        datasets: []
    };

    const usageData = {
        label: "Usage Time (Minutes)",
        backgroundColor: window.Color(internalColors.green).alpha(0.5).rgbString(),
        borderColor: internalColors.green,

        data: []
    }

    const idleData = {
        label: "Idle Time (Minutes)",
        backgroundColor: window.Color(internalColors.red).alpha(0.5).rgbString(),
        borderColor: internalColors.red,
        data: []
    }

    for (let record of records) {
        chartData.labels.push(record.applicationName);

        const productiveSeconds = (record.productiveSeconds / 60).toFixed(2);
        const idleSeconds = ((record.totalSeconds - record.productiveSeconds) / 60).toFixed(2);

        usageData.data.push(productiveSeconds);
        idleData.data.push(idleSeconds);
    }

    chartData.datasets.push(usageData);
    chartData.datasets.push(idleData);

    return chartData;
}

function mapKeyPressesForGraph(records) {
    const chartData = {
        labels: [],
        datasets: []
    };

    const usageData = {
        label: "Keys Pressed",
        backgroundColor: window.Color(internalColors.green).alpha(0.5).rgbString(),
        borderColor: internalColors.green,
        data: []
    }

    for (let record of records) {
        chartData.labels.push(record.key);
        usageData.data.push(record.count);
    }

    chartData.datasets.push(usageData);

    return chartData;
}



function mapApplicationDataForDoughnut(records) {

    const chartData = {
        datasets: [],
        labels: [],
    }


    const usageData = {
        data: [],
        backgroundColor: [],
        label: ""
    }

    const idleData = {
        data: [],
        backgroundColor: [],
        label: ""
    }

    for(let record of records)
    {
        const color = generateRandomColor();

        chartData.labels.push(record.applicationName);

        const productiveSeconds = (record.productiveSeconds / 60).toFixed(2);
        const idleSeconds = ((record.totalSeconds - record.productiveSeconds) / 60).toFixed(2);

        usageData.data.push(productiveSeconds);
        usageData.backgroundColor.push(color);
        usageData.label = "Usage Time (Minutes)";

        idleData.data.push(idleSeconds);
        idleData.backgroundColor.push(color);
        idleData.label = "Usage Time (Minutes)";
    }

    chartData.datasets.push(usageData);
    chartData.datasets.push(idleData);
    return chartData;
}

function mapMouseLocationData(records, quadrantSize = 25) {

    const chartData = {
        datasets: [],
    }

    const bubbleElement = {
        label: "Mouse Click Coordinates And Frequency",
        backgroundColor: window.Color(internalColors.green).alpha(0.5).rgbString(),
        borderColor: internalColors.green,
        borderWidth: 1,
        data: []
    }

    const maxSize = _getLargestRecordSize(records);

    for (let record of records) {

        const newRecord = {
            x: record.xPosition,
            y: record.yPosition,
            r: (record.count / maxSize * quadrantSize).toFixed(2)
        }

        bubbleElement.data.push(newRecord);
    }

    chartData.datasets.push(bubbleElement);

    return chartData;


}

function _getLargestRecordSize(records) {

    let maxValue = 1;

    for (let record of records)
        if (record.count > maxValue)
            maxValue = record.count;

    return maxValue;
}

function generateRandomColor() {
    const randomColor = '#' + Math.floor(Math.random() * 16777215).toString(16);
    return new Color(randomColor).rgbString();
}


function aggregateKeyPressCount(keyPresses) {
    var count = 0;

    for (let press of keyPresses)
        count += press.count;

    return count;
}

function aggregateMouseClicks(mouseClicks) {
    var count = 0;

    for (let click of mouseClicks)
        count += click.count;

    return count;
}
