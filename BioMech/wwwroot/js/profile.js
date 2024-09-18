const confirmModal = document.getElementById('confirmModal');
const yesBtn = document.getElementById('yesBtn');
const noBtn = document.getElementById('noBtn');
function showModal() { confirmModal.style.display = 'block'; } // Показывает окно
function hideModal() { confirmModal.style.display = 'none'; } // Скрывает окно

//Адаптация размера блока вывода фотографий при различном количестве фотографий
const resultsDiagnostics = document.getElementById('resultsDiagnostics');

const widthResultsDiagnostics = resultsDiagnostics.clientWidth

const photoList = document.querySelectorAll("#resultBlock");
const photoArray = [...photoList];
const sum = photoArray.reduce((accumulator, value) => {
    return accumulator + value.clientWidth;
}, 0);

let procentWidthResult = sum / widthResultsDiagnostics * 100
function widthResult() {
    if (procentWidthResult > 80) {
        resultsDiagnostics.classList.add('justify')
    } else {
        resultsDiagnostics.classList.remove('justify')
    }
} widthResult()

const resultPhotoBig = document.getElementById('resultPhotoBig');
const closeImgBig = document.getElementById('closeImgBig');
const bigImg = document.getElementById('bigImg');

//Увеличение фотографии при клике
$('.results__diagnostics').on("click", "img", function (event) {
    bigImg.textContent = ''
    const resultBlock = event.target
    const srcat = $(event.target).attr('src')
    console.log(srcat)
    console.log(resultBlock)
    resultPhotoBig.style.display = 'flex'
    bigImg.insertAdjacentHTML('afterbegin', `<img src="${srcat}">`);
});

document.querySelector('.close-btn').addEventListener('click', function () {
    resultPhotoBig.style.display = 'none'
});

document.addEventListener('DOMContentLoaded', function () {

    handleAlbumAllClick();
    const resultDiagnostics = document.querySelector('.results__diagnostics');

    // Обработчик для клика по крестику удаления фотографии
    resultDiagnostics.addEventListener('click', function (event) {

        if (event.target.classList.contains('remove')) {
            showModal() // Выводит окно

            // Действие при нажатии на кнопку "ДА"
            yesBtn.addEventListener('click', () => {
                const resultBlock = event.target.closest('.result');
                const photoId = resultBlock.dataset.photoId; // получение ID фотографии
                removePhoto(photoId);
                hideModal();
            });
            // Действие при нажатии на кнопку "НЕТ"
            noBtn.addEventListener('click', () => { hideModal(); });
        }
    });

    // Функция для отправки запроса на удаление фотографии по её ID
    async function removePhoto(photoId) {
        try {
            const response = await fetch(`/PersonalData/DeletePhotoDiagnostic?photoDiagnosticsId=${photoId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            // Удаление фотографии из DOM после успешного удаления
            const resultBlock = document.querySelector(`.resultBlock[data-photo-id="${photoId}"]`);
            resultBlock.parentNode.removeChild(resultBlock); widthResult()
        } catch (error) {
            console.error(error);
        }
    }
});

//Просмотр альбома всех фотографий
document.getElementById('albumAll').addEventListener('click', async function () {
    try {
        const response = await fetch('/PersonalData/GetAllDiagnostics');
        if (!response.ok) {
            throw new Error('Ошибка при запросе к контроллеру');
        }
        const data = await response.json();
        renderResults(data);
    } catch (error) {
        console.error(error);
    }
});

async function handleAlbumAllClick() {
    try {
        const response = await fetch('/PersonalData/GetAllDiagnostics');
        if (!response.ok) {
            throw new Error('Ошибка при запросе к контроллеру');
        }
        const data = await response.json();
        renderResults(data);
    } catch (error) {
        console.error(error);
    }
}

//Отображение фотографий "Лопатки"
document.getElementById('albumBlade').addEventListener('click', async function () {
    try {
        const response = await fetch('/PersonalData/GetDiagnosticsByCategory?idCategory=1'); // ID категории "Лопатки"
        if (!response.ok) {
            throw new Error('Ошибка при запросе к контроллеру');
        }
        const data = await response.json();
        renderResults(data);
    } catch (error) {
        console.error(error);
    }
});

//Отображение фотографий "Колени"
document.getElementById('albumKnees').addEventListener('click', async function () {
    try {
        const response = await fetch('/PersonalData/GetDiagnosticsByCategory?idCategory=2'); // ID категории "Колени"
        if (!response.ok) {
            throw new Error('Ошибка при запросе к контроллеру');
        }
        const data = await response.json();
        renderResults(data);
    } catch (error) {
        console.error(error);
    }
});
//Отображение фотографий "Стопы"
document.getElementById('albumFeet').addEventListener('click', async function () {
    try {
        const response = await fetch('/PersonalData/GetDiagnosticsByCategory?idCategory=3'); // ID категории "Стопы"
        if (!response.ok) {
            throw new Error('Ошибка при запросе к контроллеру');
        }
        const data = await response.json();
        renderResults(data);
    } catch (error) {
        console.error(error);
    }
});
//Отображение фотографий "Положение головы и шеи"
document.getElementById('albumNeck').addEventListener('click', async function () {
    try {
        const response = await fetch('/PersonalData/GetDiagnosticsByCategory?idCategory=5'); // ID категории "Положение шеи и головы"
        if (!response.ok) {
            throw new Error('Ошибка при запросе к контроллеру');
        }
        const data = await response.json();
        renderResults(data);
    } catch (error) {
        console.error(error);
    }
});

// Обработчик нажатия кнопки "Посмотреть все фотографии"
document.getElementById('seeAllPhotosBtn').addEventListener('click', async function () {
    try {
        const selectedCategoryId = document.querySelector('.album.active').dataset.categoryId;
        window.location.href = `/PersonalData/ProfileResults?categoryId=${selectedCategoryId}`;
    } catch (error) {
        console.error(error);
    }
});


// Обработчики нажатия кнопок категорий
document.querySelectorAll('.album').forEach(button => {
    button.addEventListener('click', function () {
        document.querySelectorAll('.album').forEach(btn => btn.classList.remove('active'));
        this.classList.add('active');
    });
});

// Обработчики нажатия кнопок оценок
document.querySelectorAll('.score-button').forEach(button => {
    button.addEventListener('click', function () {
        document.querySelectorAll('.score-button').forEach(btn => btn.classList.remove('score-button-active'));
        button.classList.add('score-button-active');
        document.getElementById('ratingInput').value = button.textContent.trim();
    });
});



// Функция для отображения результатов
function renderResults(data) {
    const resultsDiagnostics = document.querySelector('.results__diagnostics');
    resultsDiagnostics.innerHTML = '';

    if (data.error || data.length == 0) {
        const plug = document.getElementById('plug').cloneNode(true);
        if (plug) {
            plug.style.display = 'flex';
            document.getElementById('btn_see_all').style.display = 'none';
        }
        resultsDiagnostics.appendChild(plug);
        return;
    }

    // Проверка, доступен ли объект diagnosticsProblems
    if (window.diagnosticsProblems) {
        data.forEach(function (result) {
            const resultBlock = document.createElement('div');
            resultBlock.classList.add('result', 'resultBlock');

            document.getElementById('btn_see_all').style.display = 'flex';
            resultBlock.dataset.photoId = result.idPhotoDiagnostic;
            const img = document.createElement('img');
            img.src = result.photo;

            const removeBtn = document.createElement('div');
            removeBtn.classList.add('remove');
            removeBtn.textContent = '×';

            const resultData = document.createElement('p');
            resultData.classList.add('result__data');
            const date = new Date(result.dateDownload);
            resultData.textContent = date.toLocaleDateString('ru-RU', { day: '2-digit', month: '2-digit', year: 'numeric' });

            const categoryName = document.createElement('p');
            categoryName.textContent = window.diagnosticsProblems[result.problemCategoryId] || 'Нет категории';

            resultBlock.appendChild(img);
            resultBlock.appendChild(removeBtn);
            resultBlock.appendChild(resultData);
            resultBlock.appendChild(categoryName);

            resultsDiagnostics.appendChild(resultBlock);
        });
    } else {
        setTimeout(function () {
            renderResults(data);
        }, 100);
    }
}


const buttonClickList = document.querySelectorAll(".album");
const buttonClickArray = [...buttonClickList];

// Ищет айди кнопки, по которой был сделан клик и выводит сообщение
buttonClickArray.forEach(element => {
    const buttonClick = element;

    buttonClick.onclick = function () {
        buttonClick.classList.add('button-clicked');
        const activeButton = document.querySelectorAll('.button-clicked')
        activeButton.forEach(n => n !== buttonClick ? n.classList.remove('button-clicked') : null);
    };
})

const parent = document.querySelectorAll("#resultBlock");
let items = [...parent];
let wrappedItems = [];
let prevItem = [];
let currItem = [];
let obj = [];

for (const item of items) {
    currItem = item.getBoundingClientRect().top; // координаты элемента

    if (prevItem.length == 0) {
        obj.push({ 'a': [item] }); //создать новый [] и поместить элемент
    } else {
        if (prevItem < currItem) {
            wrappedItems = currItem; // то кидаем в массив
            obj.push({ 'a': [item] }); //создать новый [] и поместить элемент
        } else if (wrappedItems && wrappedItems === currItem) {
            obj[obj.length - 1].a.push(item);
        } else {
            obj[obj.length - 1].a.push(item);
        }
    }
    prevItem = currItem; // запоминаем последний
}
console.log(obj); // проверка массива

if (obj.length > 1) {
    const s = obj[0]["a"][0]
}
$('#saveDescriptionTraining').click(function (ev) {
    ev.preventDefault();
    $('#deleteForm').submit();
});


const gapDateButtonLeft = document.getElementById('gapDateButtonLeft')
const gapDateButtonRight = document.getElementById('gapDateButtonRight')
const calendarTwo = document.querySelector('.calendarTwo')
const calendarThree = document.querySelector('.calendarThree')
const date = new Date()
let year = date.getFullYear()
let month = date.getMonth() + 1

const dates = document.getElementById('dates')

const dateLeft = document.getElementById('dateLeft')
const dateRigth = document.getElementById('dateRigth')

const monthToday = document.getElementById('monthToday')
const yearToday = document.getElementById('yearToday')

let recordDate = document.getElementById('recordDate')
let recordMonth = document.getElementById('recordMonth')
let recordYear = document.getElementById('recordYear')

let dateFull;

let monthD = month;
let monthT;
let yearD = year;


getDateMonth()

//Корректировка названия месяца в 1 календаре
function usingDayStyle() {
    switch (monthD) {
        case 1:
            monthT = 'Января';
            break;
        case 2:
            monthT = 'Февраля';
            break;
        case 3:
            monthT = 'Марта';
            break;
        case 4:
            monthT = 'Апреля';
            break;
        case 5:
            monthT = 'Мая';
            break;
        case 6:
            monthT = 'Июня';
            break;
        case 7:
            monthT = 'Июля';
            break;
        case 8:
            monthT = 'Августа';
            break;
        case 9:
            monthT = 'Сентября';
            break;
        case 10:
            monthT = 'Октября';
            break;
        case 11:
            monthT = 'Ноября';
            break;
        case 12:
            monthT = 'Декабря';
            break;

    }
    let usingDay = document.querySelector('.day')

    //Форматирование даты для 1 календаря
    dateFull = `${usingDay.textContent + '.0' + monthD + '.' + yearD}`;

    document.getElementById('dateTrainingLogInput').value = dateFull;

    const calendarInput = document.getElementById('dateTrainingLogInput');
    const newRecordDiv = document.getElementById('newRecord');
    const existingRecordDiv = document.getElementById('existingRecord');

    let dateForMethod = `${yearD}-${String(monthD).padStart(2, '0')}-${String(usingDay.textContent).padStart(2, '0')}`;
    fetchTrainingLog(dateForMethod);
    function fetchTrainingLog(dateForMethod) {
        fetch(`/TrainingPrograms/GetTrainingLogs?dateTrainingLog=${dateForMethod}`)
            .then(response => {
                if (!response.ok) throw new Error('No data');
                return response.json();
            })
            .then(data => {
                displayExistingRecord(data);
            })
            .catch(error => {
                displayNewRecord();
            });
    }

    // Функция отображения формы для новой записи
    function displayNewRecord() {
        newRecordDiv.style.display = 'block';
        existingRecordDiv.style.display = 'none';
    }

    // Функция отображения существующей записи
    function displayExistingRecord(data) {
        existingRecordDiv.style.display = 'block';
        newRecordDiv.style.display = 'none';
        // Заполнение данных о тренировке
        document.querySelector('#existingRecord .h-left').textContent = `Оценка: ${data.rating}`;
        document.querySelector('#existingRecord p').textContent = data.description;
        document.getElementById('trainingLogIdInput').value = data.trainingId;

    }

    // Обработчик изменения даты в календаре
    calendarInput.addEventListener('change', function () {
        fetchTrainingLog(dateForMethod);
    });

    // Инициализация с текущей датой
    if (calendarInput.value) {

        fetchTrainingLog(dateForMethod);
    }

    // Обработчик нажатия на кнопку "Редактировать запись"
    const editButton = document.getElementById('editRecordButton');
    editButton.addEventListener('click', function () {
        existingRecordDiv.style.display = 'none';
        newRecordDiv.style.display = 'block';

        const rating = document.querySelector('#existingRecord .h-left').textContent.split(': ')[1];
        const description = document.querySelector('#existingRecord p').textContent;
        document.getElementById('ratingInput').value = rating;
        document.querySelector('textarea[name="descriptionTraining"]').value = description;

        document.querySelectorAll('.score-button').forEach(button => {
            if (button.textContent.trim() === rating) {
                button.classList.add('score-button-active');
            } else {
                button.classList.remove('score-button-active');
            }
        });
    });

    recordDate.insertAdjacentHTML('afterbegin', `
          ${usingDay.textContent}
                `);
    recordMonth.insertAdjacentHTML('afterbegin', `
          ${monthT}
                `);
    recordYear.insertAdjacentHTML('afterbegin', `
          ${yearD}
                `);

}
//Вывод названия месяца в 1 календаре
function getDateMonth() {
    if (monthD == 0) {
        monthD = 12
        yearD--
    }
    if (monthD == 13) {
        monthD = 1
        yearD++
    }
    switch (monthD) {
        case 1:
            monthT = 'Январь';
            break;
        case 2:
            monthT = 'Февраль';
            break;
        case 3:
            monthT = 'Март';
            break;
        case 4:
            monthT = 'Апрель';
            break;
        case 5:
            monthT = 'Май';
            break;
        case 6:
            monthT = 'Июнь';
            break;
        case 7:
            monthT = 'Июль';
            break;
        case 8:
            monthT = 'Август';
            break;
        case 9:
            monthT = 'Сентябрь';
            break;
        case 10:
            monthT = 'Октябрь';
            break;
        case 11:
            monthT = 'Ноябрь';
            break;
        case 12:
            monthT = 'Декабрь';
            break;

    }
    monthToday.insertAdjacentHTML('afterbegin', `
          ${monthT}
                `)
    yearToday.insertAdjacentHTML('afterbegin', `
          ${yearD}
                `);
    //Определение 1 дня недели в указанном месяце в 1 календаре
    const dayly = new Date(yearD + "-" + monthD + "-01").getDay();
    document.documentElement.style.setProperty(`--howDateOne`, `${dayly}`);


    const getDays = daysInMonth(monthD, yearD)
    function daysInMonth(monthD, yearD) {
        return new Date(yearD, monthD, 0).getDate();
    }

    //Вывод количества дней в указанном месяце в 1 календаре
    for (let i = getDays; i > 0; i--) {
        dates.insertAdjacentHTML('afterbegin', `
          <button class="calendar-buttons" id="one">${i}</button>
                `);
    }
    const oneList = document.querySelectorAll("#one");
    const oneArray = [...oneList];
    const vdgDate = String(date.getDate()).padStart(1);

    // Ищет айди кнопки, по которой был сделан клик и выводит сообщение
    oneArray.forEach(element => {
        const oneText = element.textContent;
        if (oneText === vdgDate && month === monthD && year === yearD) {
            element.classList.add('today')
            element.classList.add('day')
        } else {
            element.classList.remove('today')
            element.classList.remove('day')
        }
    })

    // Обработчики нажатия кнопок календаря
    document.querySelectorAll('.calendar-buttons').forEach(button => {
        button.addEventListener('click', function () {
            document.querySelectorAll('.calendar-buttons').forEach(btn => btn.classList.remove('day'));
            button.classList.add('day');
            recordDate.textContent = ''
            recordMonth.textContent = ''
            recordYear.textContent = ''
            document.getElementById('trainingLogIdInput').value = '';
            usingDayStyle()
        });
    });
}

//Пролистывание месяца на предыдующий
const dateLeftHandler = () => {
    monthToday.textContent = ''
    yearToday.textContent = ''
    dates.textContent = ''
    monthD--
    getDateMonth()
}
//Пролистывание месяца на следующий
const dateRightHandler = () => {
    monthToday.textContent = ''
    yearToday.textContent = ''
    dates.textContent = ''
    monthD++;
    getDateMonth()
}

usingDayStyle()


dateLeft.addEventListener('click', dateLeftHandler)
dateRigth.addEventListener('click', dateRightHandler)

const datesTwo = document.getElementById('datesTwo')

const dateLeftTwo = document.getElementById('dateLeftTwo')
const dateRigthTwo = document.getElementById('dateRigthTwo')

const monthTodayTwo = document.getElementById('monthTodayTwo')
const yearTodayTwo = document.getElementById('yearTodayTwo')

let dateFullTwo;
let monthDTwo = month;
let monthTTwo;
let yearDTwo = year;

getDateMonthTwo()

//Форматирование даты второго календаря
function usingDayStyleTwo() {
    let usingDayTwo = document.querySelector('.dayTwo')

    dateFullTwo = `${usingDayTwo.textContent + '.0' + monthDTwo + '.' + yearDTwo}`;

    gapDateButtonLeft.insertAdjacentHTML('afterbegin', `
          ${dateFullTwo}
                `);
}
//Определение месяца во втором календаре
function getDateMonthTwo() {
    if (monthDTwo == 0) {
        monthDTwo = 12
        yearDTwo--
    }
    if (monthDTwo == 13) {
        monthDTwo = 1
        yearDTwo++
    }
    switch (monthDTwo) {
        case 1:
            monthTTwo = 'Январь';
            break;
        case 2:
            monthTTwo = 'Февраль';
            break;
        case 3:
            monthTTwo = 'Март';
            break;
        case 4:
            monthTTwo = 'Апрель';
            break;
        case 5:
            monthTTwo = 'Май';
            break;
        case 6:
            monthTTwo = 'Июнь';
            break;
        case 7:
            monthTTwo = 'Июль';
            break;
        case 8:
            monthTTwo = 'Август';
            break;
        case 9:
            monthTTwo = 'Сентябрь';
            break;
        case 10:
            monthTTwo = 'Октябрь';
            break;
        case 11:
            monthTTwo = 'Ноябрь';
            break;
        case 12:
            monthTTwo = 'Декабрь';
            break;

    }
    monthTodayTwo.insertAdjacentHTML('afterbegin', `
          ${monthTTwo}
                `)
    yearTodayTwo.insertAdjacentHTML('afterbegin', `
          ${yearDTwo}
                `);
    //Определение первого дня месяца во втором календаре
    const daylyTwo = new Date(yearDTwo + "-" + monthDTwo + "-01").getDay();
    document.documentElement.style.setProperty(`--howDateTwo`, `${daylyTwo}`);


    const getDaysTwo = daysInMonthTwo(monthDTwo, yearDTwo)

    function daysInMonthTwo(monthDTwo, yearDTwo) {
        return new Date(yearDTwo, monthDTwo, 0).getDate();
    }

    //Определение количества дней в текущем месяце второго календаря
    for (let i = getDaysTwo; i > 0; i--) {
        datesTwo.insertAdjacentHTML('afterbegin', `
          <button class="calendar-buttonsTwo" id="two">${i}</button>
                `)
    }

    const twoList = document.querySelectorAll("#two");
    const twoArray = [...twoList];
    const vdgDate = String(date.getDate()).padStart(1);

    // Ищет айди кнопки, по которой был сделан клик и выводит сообщение
    twoArray.forEach(element => {
        let twoText = element.textContent;
        if (twoText == vdgDate && month === monthDTwo && year === yearDTwo) {
            element.classList.add('todayTwo')
            element.classList.add('dayTwo')
        } else {
            element.classList.remove('todayTwo')
            element.classList.remove('dayTwo')
        }
    })

    // Обработчики нажатия кнопок календаря
    document.querySelectorAll('.calendar-buttonsTwo').forEach(buttonTwo => {
        buttonTwo.addEventListener('click', function () {
            document.querySelectorAll('.calendar-buttonsTwo').forEach(btnTwo => btnTwo.classList.remove('dayTwo'));
            buttonTwo.classList.add('dayTwo');
            gapDateButtonLeft.textContent = ''
            usingDayStyleTwo()
            document.getElementById('dateTrainingLogOne').value = dateFullTwo;
        });
    });
}
//Пролистывание месяца на предыдущий во 2 календаре
const dateLeftHandlerTwo = () => {
    monthTodayTwo.textContent = ''
    yearTodayTwo.textContent = ''
    datesTwo.textContent = ''
    monthDTwo--
    getDateMonthTwo()
}
//Пролистывание месяца на следующий во 2 календаре
const dateRightHandlerTwo = () => {
    monthTodayTwo.textContent = ''
    yearTodayTwo.textContent = ''
    datesTwo.textContent = ''
    monthDTwo++;
    getDateMonthTwo()
}


usingDayStyleTwo()
document.getElementById('dateTrainingLogOne').value = dateFullTwo;

dateLeftTwo.addEventListener('click', dateLeftHandlerTwo)
dateRigthTwo.addEventListener('click', dateRightHandlerTwo)

const datesThree = document.getElementById('datesThree')

const dateLeftThree = document.getElementById('dateLeftThree')
const dateRigthThree = document.getElementById('dateRigthThree')

const monthTodayThree = document.getElementById('monthTodayThree')
const yearTodayThree = document.getElementById('yearTodayThree')

let dateFullThree;


let monthDThree = month;
let monthTThree;
let yearDThree = year;

getDateMonthThree()

//Форматирование даты в 3 календаре
function usingDayStyleThree() {
    let usingDayThree = document.querySelector('.dayThree')

    dateFullThree = `${usingDayThree.textContent + '.0' + monthDThree + '.' + yearDThree}`;

    gapDateButtonRight.insertAdjacentHTML('afterbegin', `
          ${dateFullThree}
                `);
}
//Определение названия выбранного месяца в 3 календаре
function getDateMonthThree() {
    if (monthDThree == 0) {
        monthDThree = 12
        yearDThree--
    }
    if (monthDThree == 13) {
        monthDThree = 1
        yearDThree++
    }
    switch (monthDThree) {
        case 1:
            monthTThree = 'Январь';
            break;
        case 2:
            monthTThree = 'Февраль';
            break;
        case 3:
            monthTThree = 'Март';
            break;
        case 4:
            monthTThree = 'Апрель';
            break;
        case 5:
            monthTThree = 'Май';
            break;
        case 6:
            monthTThree = 'Июнь';
            break;
        case 7:
            monthTThree = 'Июль';
            break;
        case 8:
            monthTThree = 'Август';
            break;
        case 9:
            monthTThree = 'Сентябрь';
            break;
        case 10:
            monthTThree = 'Октябрь';
            break;
        case 11:
            monthTThree = 'Ноябрь';
            break;
        case 12:
            monthTThree = 'Декабрь';
            break;

    }
    monthTodayThree.insertAdjacentHTML('afterbegin', `
          ${monthTThree}
                `)
    yearTodayThree.insertAdjacentHTML('afterbegin', `
          ${yearDThree}
                `);

    //Определение 1 дня в указанном месяце в 3 календаре
    const daylyThree = new Date(yearDThree + "-" + monthDThree + "-01").getDay();
    document.documentElement.style.setProperty(`--howDateThree`, `${daylyThree}`);


    const getDaysThree = daysInMonthThree(monthDThree, yearDThree)
    function daysInMonthThree(monthDThree, yearDThree) {
        return new Date(yearDThree, monthDThree, 0).getDate();
    }

    //Определение количества дней в указанном месяце в 3 календаре
    for (let i = getDaysThree; i > 0; i--) {
        datesThree.insertAdjacentHTML('afterbegin', `
          <button class="calendar-buttonsThree" id="three">${i}</button>
                `)
    }

    const threeList = document.querySelectorAll("#three");
    const threeArray = [...threeList];
    const vdgDate = String(date.getDate()).padStart(1);

    // Ищет айди кнопки, по которой был сделан клик и выводит сообщение
    threeArray.forEach(element => {
        let threeText = element.textContent;
        if (threeText == vdgDate && month === monthDThree && year === yearDThree) {
            element.classList.add('todayThree')
            element.classList.add('dayThree')
        } else {
            element.classList.remove('todayThree')
            element.classList.remove('dayThree')
        }
    })

    // Обработчики нажатия кнопок календаря
    document.querySelectorAll('.calendar-buttonsThree').forEach(buttonThree => {
        buttonThree.addEventListener('click', function () {
            document.querySelectorAll('.calendar-buttonsThree').forEach(btnThree => btnThree.classList.remove('dayThree'));
            buttonThree.classList.add('dayThree');
            gapDateButtonRight.textContent = ''
            usingDayStyleThree()
            document.getElementById('dateTrainingLogTwo').value = dateFullThree;


        });
    });

}
//Пролистывание месяца на предыдущий в 3 календаре
const dateLeftHandlerThree = () => {
    monthTodayThree.textContent = ''
    yearTodayThree.textContent = ''
    datesThree.textContent = ''
    monthDThree--
    getDateMonthThree()
}
//Пролистывание месяца на следующий в 3 календаре
const dateRightHandlerThree = () => {
    monthTodayThree.textContent = ''
    yearTodayThree.textContent = ''
    datesThree.textContent = ''
    monthDThree++;
    getDateMonthThree()
}

usingDayStyleThree()
document.getElementById('dateTrainingLogTwo').value = dateFullThree;

dateLeftThree.addEventListener('click', dateLeftHandlerThree)
dateRigthThree.addEventListener('click', dateRightHandlerThree)

//Выбор первой даты для графика
const gapDateButtonLeftdHandler = () => {
    if (calendarTwo.style.display === 'inline-grid') {
        calendarTwo.style.display = 'none'
    } else {
        calendarTwo.style.display = 'inline-grid'
        calendarThree.style.display = 'none'
    }
}

//Выбор второй даты для графика
const gapDateButtonRightHandler = () => {
    if (calendarThree.style.display === 'inline-grid') {
        calendarThree.style.display = 'none'
    } else {
        calendarThree.style.display = 'inline-grid'
        calendarTwo.style.display = 'none'
    }
}

gapDateButtonLeft.addEventListener('click', gapDateButtonLeftdHandler)
gapDateButtonRight.addEventListener('click', gapDateButtonRightHandler)

const estimateDays = document.getElementById('estimateDays')
const estimateAverage = document.getElementById('estimateAverage')
const graphCanvas = document.getElementById('graphCanvas')
const canvasClick = document.querySelector('.canvas-click')
const gap = document.querySelector('.gap')

//Появление графика "Оценка по дням" при нажатии на кнопку "обновить данные графика
graphCanvas.insertAdjacentHTML('afterbegin', `<canvas class="canvas-click" id="speedChart"></canvas>`);

const estimateDaysHandler = () => {
    gap.style.display = 'flex'
    estimateAverage.classList.remove('active__acc')
    estimateDays.classList.add('active__acc')
    graphCanvas.textContent = ''
}

document.getElementById('UpdateGraph').addEventListener('click', function() {
    var startDate = document.getElementById('dateTrainingLogOne').value;
    var endDate = document.getElementById('dateTrainingLogTwo').value;

    var formattedStartDate = startDate.split('.').reverse().join('-');
    var formattedEndDate = endDate.split('.').reverse().join('-');

    var xhr = new XMLHttpRequest();
    xhr.open('GET', '/TrainingPrograms/GetTrainingRating?dateTrainingLogOne=' + formattedStartDate + '&dateTrainingLogTwo=' + formattedEndDate, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                var ratings = JSON.parse(xhr.responseText);
                graphCanvas.textContent = ''
                graphCanvas.insertAdjacentHTML('afterbegin', `<canvas class="canvas-click" id="speedChart"></canvas>`);
                speedCanvasGraph(ratings);
            } else {
                console.error('Ошибка при выполнении запроса:', xhr.status);
            }
        }
    };
    xhr.send();
});


//Появление графика "Средняя оценка"
const estimateAverageHandler = () => {
    gap.style.display ='none'
    estimateDays.classList.remove('active__acc')
    estimateAverage.classList.add('active__acc')
    graphCanvas.textContent = ''
    graphCanvas.insertAdjacentHTML('afterbegin', `<canvas class="canvas-click" id="densityChart"></canvas>`);
    densityChartGraph()
}

estimateDays.addEventListener('click', estimateDaysHandler)
estimateAverage.addEventListener('click', estimateAverageHandler)
