
//Настройка графиков
function speedCanvasGraph(ratings) {
    var speedCanvas = document.getElementById("speedChart");

    Chart.defaults.font.family = "Lato";
    Chart.defaults.font.size = 18;

    console.log(ratings);

    var speedData = {
        labels: ratings.datesMark,
        datasets: [{
            label: "Оценка дня",
            data: ratings.ratingsMark,
            lineTension: 0,
            fill: false,
            borderColor: 'orange',
            backgroundColor: 'transparent',
            borderDash: [5, 5],
            pointBorderColor: 'orange',
            pointBackgroundColor: 'rgba(255,150,0,0.5)',
            pointRadius: 5,
            pointHoverRadius: 10,
            pointHitRadius: 30,
            pointBorderWidth: 2,
            pointStyle: 'rectRounded'
        }]
    };

    var chartOptions = {
        legend: {
            display: true,
            position: 'top',
            labels: {
                boxWidth: 10,
                fontColor: 'black'
            }
        },
        scales: {
            y: {
                suggestedMin: 0,
                suggestedMax: 10,
                ticks: {
                    stepSize: 1
                }
            }
        }
    }

    var lineChart = new Chart(speedCanvas, {
        type: 'line',
        data: speedData,
        options: chartOptions
    });
}

function densityChartGraph() {
    var densityCanvas = document.getElementById("densityChart");

    Chart.defaults.font.family = "Lato";
    Chart.defaults.font.size = 18;

    $.ajax({
        url: '/TrainingPrograms/GetAverageRatingByMonth',
        type: 'GET',
        success: function (data) {
            var months = Object.keys(data);
            var ratings = Object.values(data);

            var densityData = {
                label: 'Средняя оценка за месяц',
                data: ratings,
                backgroundColor: createGradient(densityCanvas.getContext('2d')),
                borderColor: 'rgba(0, 0, 0, 0)',
                borderWidth: 2,
                hoverBorderWidth: 0,
            };

            var chartOptions = {
                scales: {
                    y: {
                        suggestedMin: 0,
                        suggestedMax: 10,
                        ticks: {
                            stepSize: 1
                        }
                    }
                },
                elements: {
                    rectangle: {
                        borderSkipped: 'left',
                    }
                }
            };

            var barChart = new Chart(densityCanvas, {
                type: 'bar',
                data: {
                    labels: months,
                    datasets: [densityData],
                },
                options: chartOptions
            });
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function createGradient(context) {
    var gradient = context.createLinearGradient(0, 0, 0, 400);
    gradient.addColorStop(0.5, 'rgba(121, 85, 72, 0.6)');
    gradient.addColorStop(1, 'rgb(242, 230, 219)'); 
    return gradient;
}


