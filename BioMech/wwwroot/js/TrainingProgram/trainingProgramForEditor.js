document.addEventListener('DOMContentLoaded', function () {
    const dayButtons = document.querySelectorAll('.day-block');
    const dayTitle = document.querySelector('.h-left');
    const trainingContainer = document.getElementById('training-container');
    const recoverySection = document.getElementById('recovery-section');
    const markAsDoneButton = document.getElementById('markAsDoneButton');

    let dayIndex = 1; // Индекс для сохранения номера дня

    const queryParams = new URLSearchParams(window.location.search);
    const trainingProgramId = queryParams.get('trainingProgramID');

    let currentDayId = dayButtons.length > 0 ? dayButtons[0].getAttribute('data-id') : null;

    const currentDateElem = document.getElementById('currentDate');
    const prevDayButton = document.getElementById('prevDay');
    const nextDayButton = document.getElementById('nextDay');

    const startButton = document.querySelector('form button.more'); // Кнопка "Начать выполнение"
    const controlButtons = document.querySelectorAll('.program-day-buttons button'); // Кнопки управления датами и выполнением

    let currentDate = new Date();

    let formattedDate = formatDate(currentDate);


    //Форматирование даты
    function formatDate(date) {
        let day = date.getDate();
        let month = date.getMonth() + 1;
        let year = date.getFullYear();

        day = (day < 10) ? '0' + day : day;
        month = (month < 10) ? '0' + month : month;

        return `${day}.${month}.${year}`;
    }

    //Отметка активного дня
    function setActive(button) {
        dayButtons.forEach(b => b.classList.remove('day-block-active'));
        button.classList.add('day-block-active');
        const day = button.getAttribute('data-day');
        dayTitle.textContent = day;

        if (button.textContent.includes("Выходной")) {
            trainingContainer.style.display = 'none';
            recoverySection.style.display = 'flex';
        } else {
            trainingContainer.style.display = 'block';
            recoverySection.style.display = 'none';
        }
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
            fetchTrainings(currentDayId); 
        });
    });


});

