document.addEventListener('DOMContentLoaded', function () {
    const dayButtons = document.querySelectorAll('.day-block');
    const dayTitle = document.querySelector('.h-left');
    const trainingContainer = document.getElementById('training-container');
    const recoverySection = document.getElementById('recovery-section');
    const markAsDoneButton = document.getElementById('markAsDoneButton');

    let dayIndex = 1; // Индекс для сохранения номера дня

    const queryParams = new URLSearchParams(window.location.search);
    const trainingProgramId = queryParams.get('trainingProgramID');

    const userId = document.getElementById('userId').value;

    let currentDayId = dayButtons.length > 0 ? dayButtons[0].getAttribute('data-id') : null;

    const currentDateElem = document.getElementById('currentDate');
    const prevDayButton = document.getElementById('prevDay');
    const nextDayButton = document.getElementById('nextDay');

    const startButton = document.querySelector('form button.more'); // Кнопка "Начать выполнение"
    const controlButtons = document.querySelectorAll('.program-day-buttons button'); // Кнопки управления датами и выполнением

    let currentDate = new Date();

    let formattedDate = formatDate(currentDate); 

    checkTrainingProgramState();
    markCompletedPassingDays();

    //Форматирование даты
    function formatDate(date) {
        let day = date.getDate();
        let month = date.getMonth() + 1;
        let year = date.getFullYear();

        day = (day < 10) ? '0' + day : day;
        month = (month < 10) ? '0' + month : month;

        return `${day}.${month}.${year}`;
    }

    //Обновление отображенной даты
    function updateDateDisplay() {
        const options = { day: 'numeric', month: 'long' };
        formattedDate = formatDate(currentDate);
        currentDateElem.textContent = currentDate.toLocaleDateString('ru-RU', options);
    }

    updateDateDisplay();

    //Пролистывание дней на день назад
    prevDayButton.addEventListener('click', function () {
        event.preventDefault();
        if (!markAsDoneButton.disabled) {
            currentDate.setDate(currentDate.getDate() - 1);
            updateDateDisplay();
        }
    });

    //Пролистывание дней на день вперёд
    nextDayButton.addEventListener('click', function () {
        event.preventDefault();
        if (!markAsDoneButton.disabled) {
            currentDate.setDate(currentDate.getDate() + 1);
            updateDateDisplay();
        }
    });

    //Запуск старта тренировки
    startButton.addEventListener('click', function (event) {
        event.preventDefault();
        toggleTrainingProgram();
    });

    //Функция старта тренировки
    function toggleTrainingProgram() {
        fetch(`/TrainingPrograms/ToggleTrainingProgram?userId=${userId}&trainingProgramId=${trainingProgramId}&checkOnly=true`)
            .then(response => response.json())
            .then(data => {
                if (data.isStarted) {
                    fetch(`/TrainingPrograms/UpdateStatusTrainingLogs`, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    })
                        .then(updateResponse => {
                            if (updateResponse.ok) {
                                updateProgramState();
                                location.reload();
                            } else {
                                console.error('Ошибка при обновлении статуса логов');
                                
                            }
                        });
                } else {
                    updateProgramState();
                }
            }).catch(error => console.error('Error:', error));

        //Обновление статуса программы тренировок
        function updateProgramState() {
            fetch(`/TrainingPrograms/ToggleTrainingProgram?userId=${userId}&trainingProgramId=${trainingProgramId}`)
                .then(response => response.json())
                .then(data => {
                    startButton.textContent = data.buttonText; 
                    if (data.isStarted) {
                        controlButtons.forEach(button => {
                            button.disabled = false;
                            button.style.opacity = '1';
                        });
                    } else {
                        controlButtons.forEach(button => {
                            button.disabled = true;
                            button.style.opacity = '0.5';
                        });
                    }
                });
        }
    }

    //Проверка статуса тренировочной программы
    function checkTrainingProgramState() {
        fetch(`/TrainingPrograms/ToggleTrainingProgram?userId=${userId}&trainingProgramId=${trainingProgramId}&checkOnly=true`)
            .then(response => response.json())
            .then(data => {
                updateUI(data);
            }).catch(error => console.error('Error:', error));
    }

    function updateUI(data) {
        startButton.textContent = data.buttonText;
        controlButtons.forEach(button => {
            button.disabled = !data.isStarted;
            button.style.opacity = data.isStarted ? '1' : '0.5';
        });
    }

    //Проверка номера дня
    dayButtons.forEach(button => {
        if (button.textContent.includes("Выходной")) {
            button.textContent = `Выходной день`;
            button.setAttribute('data-day', `День ${dayIndex}`);
        } else {
            button.textContent = `День ${dayIndex}`;
            button.setAttribute('data-day', `День ${dayIndex}`);
        }
        button.setAttribute('data-index', dayIndex);
        dayIndex++;
    });


    //Форматирование даты
    function formatToDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('ru-RU', { day: 'numeric', month: 'long' });
    }

    //Отметка дня как выполненый
    function setActive(button) {
        dayButtons.forEach(b => b.classList.remove('day-block-active'));
        button.classList.add('day-block-active');
        const day = button.getAttribute('data-day');
        dayTitle.textContent = day; 

        if (button.classList.contains('completed')) {
            markAsDoneButton.textContent = 'Выполнено';
            markAsDoneButton.disabled = true;
            currentDateElem.textContent = formatToDate(button.dataset.dateLog);
            
        } else {
            markAsDoneButton.textContent = 'Отметить как выполненное';
            markAsDoneButton.disabled = false;
            updateDateDisplay();
        }

        if (button.textContent.includes("Выходной")) {
            trainingContainer.style.display = 'none';
            recoverySection.style.display = 'flex';
        } else {
            trainingContainer.style.display = 'block';
            recoverySection.style.display = 'none';
        }
    }


    //Отметка дня программы как выполненная
    function markCompletedDays() {
        fetch(`/TrainingPrograms/GetTrainingLogs`)
            .then(response => response.json())
            .then(data => {
                let firstCompletedDaySet = false;
                data.forEach(log => {
                    if (log.statusDone) {
                        const completedDayButton = document.querySelector(`button[data-id="${log.oneDayTrainingProgramId}"]`);
                        completedDayButton.classList.add('completed');
                        completedDayButton.dataset.dateLog = log.dateTrainingLog;
                        if (!firstCompletedDaySet) {
                            setActive(completedDayButton); // Активировать первую выполненную тренировку
                            firstCompletedDaySet = true;
                        }
                    }
                });
                if (!firstCompletedDaySet && dayButtons.length > 0) {
                    setActive(dayButtons[0]); // Если нет выполненных, активировать первую кнопку
                }
            })
            .catch(error => console.error('Error:', error));
    }

    function markCompletedPassingDays() {
        fetch(`/TrainingPrograms/GetTrainingInfoPassingDays`)
            .then(response => response.json())
            .then(data => {
                let firstCompletedDaySet = false;
                data.forEach(log => {
                    const completedDayButton = document.querySelector(`button[data-id="${log.oneDayTrainingProgramId}"]`);
                    if (completedDayButton) {
                        completedDayButton.classList.add('completed');
                        completedDayButton.dataset.dateLog = log.dateTrainingLog;
                        if (!firstCompletedDaySet) {
                            setActive(completedDayButton);
                            firstCompletedDaySet = true;
                        }
                    }
                });
                if (!firstCompletedDaySet && dayButtons.length > 0) {
                    setActive(dayButtons[0]);
                }
            })
            .catch(error => console.error('Error:', error));
    }

    //Отмена отметки программы как выполненная
    markAsDoneButton.addEventListener('click', function () {

        if (startButton.textContent === 'Начать выполнение программы') {
            return; 
        }

        if (this.disabled) {
            return;
        }

        var dateString = document.getElementById('currentDate').textContent;
        function convertDateToISO(dateStr) {
            let [day, monthName] = dateStr.split(' ');
            const monthNames = { 'января': '01', 'февраля': '02', 'марта': '03', 'апреля': '04', 'мая': '05', 'июня': '06', 'июля': '07', 'августа': '08', 'сентября': '09', 'октября': '10', 'ноября': '11', 'декабря': '12' };
            let month = monthNames[monthName];
            let year = new Date().getFullYear();

            return `${year}-${month}-${day}`;
        }

        var formattedDate = convertDateToISO(dateString);

        fetch(`/TrainingPrograms/PostPassingOneDayProgramTrainingLog?dateTrainingLog=${formattedDate}&oneDayTrainingProgramID=${currentDayId}`, {
            method: 'POST'
        }).then(response => {
            if (response.ok) {
                let selectedButton = document.querySelector(`button[data-id="${currentDayId}"]`);
                selectedButton.classList.add('completed');
                markAsDoneButton.textContent = 'Выполнено';
                markAsDoneButton.disabled = true;

                markCompletedDays();
            } else {
                
            }
        }).catch(error => {
            console.error('Ошибка:', error);
        });

    });

    function fetchTrainings(dayId) {
        fetch(`/TrainingPrograms/GetTrainingDetails?dayTrainingProgram=${dayId}`)
            .then(response => response.json())
            .then(trainings => {
                trainingContainer.innerHTML = '';
                trainings.forEach(training => {
                    const trainingElement = `
                         <div class="program-day-training">
                            <a style="text-decoration: none;" href="/TrainingPrograms/ProgramVideo?idTrainingProgram=${training.idTraining}" class="text-head">
                                <h3 class="h-brown">${training.nameTraining}</h3>
                            </a>
                            <div class="training-description">
                                <div class="video-training-block">
                                    <a href="/TrainingPrograms/ProgramVideo?idTrainingProgram=${training.idTraining}">
                                        <img class="video-training" src="${training.photo_Training_Video}" alt="" />
                                    </a>
                                </div>
                                <div class="training-description-text">
                                    <a style="text-decoration: none;" href="/TrainingPrograms/ProgramVideo?idTrainingProgram=${training.idTraining}" class="text-open">
                                        <h3 class="h-brown">${training.nameTraining}</h3>
                                    </a>
                                    <p>${training.descriptionTraining}</p>
                                </div>
                            </div>
                        </div>`;
                    trainingContainer.insertAdjacentHTML('beforeend', trainingElement);
                });
            })
            .catch(error => console.error('Error loading the trainings data:', error));
    }

    // Устанавка активной кнопки при загрузке
    if (dayButtons.length > 0) {
        setActive(dayButtons[0]);
        fetchTrainings(dayButtons[0].getAttribute('data-id')); // Загрузка данных для первого дня при загрузке страницы
    }

    dayButtons.forEach(button => {
        button.addEventListener('click', function () {
            setActive(this); 
            currentDayId = this.getAttribute('data-id');
            fetchTrainings(currentDayId); // Выполнение запроса к серверу и обновление данных на странице
        });
    });


});

