const confirmModal = document.getElementById('confirmModal');
const yesBtn = document.getElementById('yesBtn');
const noBtn = document.getElementById('noBtn');

function showModal() { confirmModal.style.display = 'block'; } // Показывает окно
function hideModal() { confirmModal.style.display = 'none'; } // Скрывает окно


//Функция вызова окна для уточнения, удалить аккаунт или нет
$('#autorizationButton').submit(function (ev) {
    if (validation(this) === true) {
        ev.preventDefault();
        var password = document.querySelector('input[name="password"]').value; // Получить значение из поля ввода

        fetch('/Authentication/Registration', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: `password=${encodeURIComponent(password)}`
        })
            .then(response => {
                if (response.ok) {
                    
                } else {
                    alert('увы');
                }
            })
            .catch(error => {
                alert("Ошибка при обновлении почты: " + error.message);
            });


    } else {
        ev.preventDefault()
    }
});