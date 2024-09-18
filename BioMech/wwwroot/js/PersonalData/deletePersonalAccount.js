const confirmModal = document.getElementById('confirmModal');//обнаруживает окно
const yesBtn = document.getElementById('yesBtn');
const noBtn = document.getElementById('noBtn'); //Кнопки взаимодействия в окне

function showModal() { confirmModal.style.display = 'block'; } // Показывает окно
function hideModal() { confirmModal.style.display = 'none'; } // Скрывает окно


//Функция удаления аккаунта
$('#deleteForm').submit(function (ev) {
    if (validation(this) === true) { //Проверка правильности валидации
        ev.preventDefault(); //Попытка остановить перезагрузку
        var password = document.querySelector('input[name="checkPasswordForDelete"]').value; //Получение значения текста пароля

        fetch('/Authentication/Registration', { //Берёт пароль из указанного тобою контроллера
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: `password=${encodeURIComponent(password)}`
        })
            .then(response => { //С этого момента начинается проблема
                if (response.ok) { //Если пароль совпадает
                    showModal() //Вызов окна
                    yesBtn.addEventListener('click', () => { //В случае клика по "Да"
                        hideModal(); //Скрывает окно
                        $('#deleteForm').unbind('submit').submit(); // Возвращает отправку формы
                    });
                    noBtn.addEventListener('click', () => { hideModal(); }); //"Нет", скрывает окно
                } else { //Если пароль не совпадает
                    alert('Неверный пароль');
                }
            })
            .catch(error => {
                alert("Ошибка при обновлении пароля: " + error.message); //Вывод ошибки в виде окна
            });
    } else {
        ev.preventDefault() //В случае неверной валидации, указывается ошибка и также не перезагружает
    }
});
