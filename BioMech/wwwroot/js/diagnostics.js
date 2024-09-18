

// Функция для обновления текста в модальном окне с результатами
function updateResultText(modelType, response) {
    const resultTextElement = document.getElementById('resultText');
    let resultText = '';
    let trainingProgramID;

    switch (modelType) {
        case 'ShoulderBlades':
            resultText += response.labelUrl ? 'Обнаружены крыловидные лопатки.' : 'Крыловидные лопатки не обнаружены.';
            trainingProgramID = response.labelUrl ? 1 : 1;
            break;
        case 'FootBone':
            resultText += response.labelUrl ? 'Обнаружена вальгусная деформация стопы.' : 'Вальгусная деформация стопы не обнаружена.';
            trainingProgramID = response.labelUrl ? 3 : 3;
            break;
        case 'NeckProtraction':
            var degreeAngle = response.degreeAngle;
            if (degreeAngle !== null) {
                degreeAngle = parseInt(degreeAngle);
                if (degreeAngle > 7) {
                    resultText += 'Обнаружена протракция шеи (компьютерная шея).';
                    trainingProgramID = 4;
                } else if (degreeAngle < 7) {
                    resultText += 'Не обнаружена протракция шеи (компьютерная шея).';
                    trainingProgramID = 4;
                }
                else {
                    resultText += 'Произошла ошибка. Попробуйте сделать и загрузить другую фотографию.';
                }

            }
            else {
                resultText += 'Произошла ошибка. Попробуйте сделать и загрузить другую фотографию.';
            }
            break;
        case 'KneesProblems':
            var degreeAngle = response.degreeAngle;
            if (degreeAngle !== null) {
                degreeAngle = parseInt(degreeAngle);
                if (degreeAngle <= 174) {
                    resultText += 'Обнаружен вальгус колен.';
                    trainingProgramID = 2;
                } else if (degreeAngle >= 182) {
                    resultText += 'Обнаружен варус колен';
                    trainingProgramID = 2;
                }
                else if (degreeAngle < 182 && degreeAngle > 174) {
                    resultText += 'Не обнаружен вальгус или варус колен.';
                    trainingProgramID = 2;
                }
                else {
                    resultText += 'Произошла ошибка. Попробуйте сделать и загрузить другую фотографию.';
                }
                break;
            }
            else {
                resultText += 'Произошла ошибка. Попробуйте сделать и загрузить другую фотографию.';

            }
            break;
            
    }

    resultTextElement.textContent = resultText;
    if (trainingProgramID && startTrainingButton) {
        startTrainingButton.setAttribute('data-training-program-id', trainingProgramID);
    }
}


//Функция для удобного создания кнопок 
const element = (tag, classes = [], content) => {
    const node = document.createElement(tag)
    if (classes.length) {
        node.classList.add(...classes)
    }
    if (content) {
        node.textContent = content
    }

    return node
}
const answer = element('button', ['more'], 'Получить ответ')

const progress = document.getElementById('progress');

// Проверка статуса авторизованного пользователя
function checkAuthorizationAndProceed(callback) {
    fetch('/authentication/CheckAuthorization')
        .then(response => response.json())
        .then(data => {
            if (data.isAuthorized) {
                callback(); // Продолжение выполнения действия, если пользователь авторизован
            } else {
                window.location.href = '/authentication/Authorization'; // Перенаправление на страницу авторизации
            }
        });
}

//Функция запуска открытия фотографии
function upload(options = {}) {
    const modalImg = document.getElementById('modalImg');
    const input = document.getElementById('file');

    const load = element('button', ['more'], 'Загрузить фотографию')

    if (options.accept && Array.isArray(options.accept)) {
        input.setAttribute('accept', options.accept.join(','))
    }

    input.insertAdjacentElement('afterend', answer)
    input.insertAdjacentElement('afterend', load)
    answer.style.display = 'none'
    progress.style.display='none'

    const triggerInput = () => input.click()

    //Функция при нажатии на "Загрузить фотографию"
    const changeHandler = event => {
        if (!event.target.files.length) {
            return
        }
        
        const files = Array.from(event.target.files)
        modalImg.innerHTML = '' //Обнуление контента в блоке для фотографии

        //Сохранение фотографии
        files.forEach(file => {
            if (!file.type.match('image')) {
                return
            }

            const reader = new FileReader()

            //Где конкретно будет сохраняться фотография и как
            reader.onload = ev => {
                const src = ev.target.result
                modalImg.insertAdjacentHTML('afterbegin', `
                    <img src="${src}" alt="${file.name}"/>
                `)
                answer.style.display = 'block' //Появление "Получить ответ" после появления фотографии
            }

            reader.readAsDataURL(file)
        })
    }

    let interval;

    //Обновление процесса прогресса работы модели
    const updateProgress = (percentage) => {
        const progressElement = document.querySelector('.progress__true');
        const infoElement = document.querySelector('.progress__info');

        progressElement.style.width = `${percentage}%`;
        infoElement.textContent = `${percentage}%`;
    };


    //Старт процесса прогресса работы модели
    const startProgressAnimation = () => {
        let progress = 0;
        const maxProgressBeforeFinish = 99; // Максимум до получения ответа от сервера

        if (interval) clearInterval(interval);

        interval = setInterval(() => {
            if (progress < maxProgressBeforeFinish) {
                progress += 1;
                updateProgress(progress);
            }
        }, 130); 
    };

    // Функция для быстрого завершения анимации
    const finishProgressAnimation = () => {
        clearInterval(interval); // Останановка анимации
        updateProgress(100); // Устанавка прогресс на 100%
    };

    //Функция для появления кнопки "Получить ответ", при вызове функции сразу появляется прогресс
    const answerHandler = () => {
        progress.style.display = 'block';
        disableButtons(true);
        startProgressAnimation();

        const fileInput = document.getElementById('file');
        if (fileInput.files.length === 0) {
            
            progress.style.display = 'none';
            clearInterval(interval);
            return;
        }

        const formData = new FormData();
        formData.append('photoForModel', fileInput.files[0]);
        formData.append('modelType', modelType);

        fetch(url, {
            method: 'POST',
            body: formData,
        })
            .then(response => {
                if (response.ok) {

                    return response.json(); 
                }
                throw new Error('Ошибка при отправке фотографии.');
            })
            .then(data => {
                const imageUrl = data.imageUrl;
                console.log(imageUrl);
                finishProgressAnimation();
                progress.style.display = 'none';
                const resultImgDiv = document.getElementById('resultImg');
                resultImgDiv.innerHTML = `<img src="${imageUrl}" alt=""/>`;
                updateResultText(modelType, data);
                hideModal()
                showResultModal();
            })
            .catch(error => {
                finishProgressAnimation();
                progress.style.display = 'none';
                const resultTextElement = document.getElementById('resultText');
                resultTextElement.textContent = "Произошла ошибка. Пожалуйста, попробуйте ещё раз позже или загрузите другую фотографию.";

                const startTrainingButton = document.getElementById('startTrainingButton');
                if (startTrainingButton) {
                    startTrainingButton.style.display = 'none'; // Скрытие кнопки
                }

                hideModal()
                showResultModal();
            })
            .finally(() => {
                disableButtons(false);  // Включение кнопок
            });
    };

    answer.addEventListener('click', answerHandler)
    load.addEventListener('click', triggerInput)
    input.addEventListener('change', changeHandler)
}

upload({
    accept: ['.png', '.jpg', '.jpeg', '.gif']
})

function disableButtons(disable) {
    const buttons = document.querySelectorAll('button');
    buttons.forEach(button => {
        button.disabled = disable;
        button.classList.toggle('disabled', disable);
    });
}


const openModalButton = document.getElementById('openModal');
const closeButton = document.getElementById('close-btn');
const closeButtonResult = document.getElementById('close-btn-result');
const confirmModal = document.getElementById('confirmModal');
const resultModal = document.getElementById('resultModal');
function showModal() { confirmModal.style.display = 'flex'; }
function showResultModal() { resultModal.style.display = 'flex'; }
function hideResultModal() { resultModal.style.display = 'none'; }
function hideModal() { confirmModal.style.display = 'none'; }

//Функции появления и исчезновения блоков
const openModalHandler = () => {
    showModal()
}
const closeHandler = () => {
    hideModal()
    hideResultModal()
    modalImg.innerHTML = ''
    answer.style.display = 'none'
    progress.style.display = 'none'

}
openModalButton.addEventListener('click', function () {
    checkAuthorizationAndProceed(() => {
        openModalHandler();
    });
});

//Функция запуска тренировочной программы при клике на кнопке "Начать тренироваться"
document.addEventListener('DOMContentLoaded', function () {
    const trainingButton = document.getElementById('startTrainingButton');
    if (trainingButton) {
        trainingButton.addEventListener('click', function (event) {
            event.preventDefault();
            const trainingProgramID = this.getAttribute('data-training-program-id');
            const url = `/TrainingPrograms/ProgramsCorrection?trainingProgramID=${trainingProgramID}`;
            window.location.href = url;
        });
    }
});



closeButton.addEventListener('click', closeHandler)
closeButtonResult.addEventListener('click', closeHandler)

//Настройка слайдера фотографий
new Swiper('.image-slider', {
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev'
    },
    pagination: {
        el: '.swiper-pagination',
        clickable: true,
        dynamicBullets: true
    },
    hashNavigation: {
        watchState: true,
    },
    keyboard: {
        enabled: true,
        onlyInViewport: true
    },

    slidesPerView: 3,

    centeredSlides: true,
    loop: true,

    speed: 500,

    effect: 'coverflow',

    coverflowEffect: {
        rotate: 10,
        stretch: -10,
        slideShadows: false,
    }
});