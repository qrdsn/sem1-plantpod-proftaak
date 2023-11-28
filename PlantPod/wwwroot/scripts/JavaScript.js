function ShowGraph(...dataToDisplay) {
    var chart = document.getElementById(dataToDisplay[2]).getContext('2d');
    var data = {
        type: 'line',
        data: {
            labels: [],
            datasets: [{
                label: dataToDisplay[3],
                data: []
            }]
        },
        options: {
            plugins: {
                legend: {
                    display: false
                }
            },
            scales: {
                y: {
                    min: 0,
                    max: 1023
                }
            }
        }
    }
    for (const label of dataToDisplay[0]) {
        data['data']['labels'].push(label);
    }
    for (const readingdata of dataToDisplay[1]) {
        data['data']['datasets'][0]['data'].push(readingdata);
    }
    var mychart = new Chart(chart, data);
}
function showNavmenu(onOrOff) {
    var siderbarNavmenu = document.getElementById("sidebarNavmenu");
    if (siderbarNavmenu !== null && onOrOff == false) {
        siderbarNavmenu.style.display = "none";
    }
    else if (siderbarNavmenu !== null && onOrOff == true) {
        siderbarNavmenu.style.display = "initial";
    }
}