const confirmMassage = document.getElementById('confirmMassage');
const confirmErrorMassage = document.getElementById('confirmErrorMassage');

function showMassage() { confirmMassage.style.display = 'block'; }// Показывает сообщение
function hideMassage() { confirmMassage.style.display = 'none'; }
function showErrorMassage() { confirmErrorMassage.style.display = 'block'; }// Показывает сообщение ошибки
function hideErrorMassage() { confirmErrorMassage.style.display = 'none'; }

//Функция создания заявки в поддержку
document.body.addEventListener('click', function (event) {
    if (event.target.id === 'supportMassage') {
        const userName = document.querySelector('input[name="userName"]').value;
        const userEmail = document.querySelector('input[name="userEmail"]').value;
        const userMessage = document.querySelector('textarea[name="userMessage"]').value;


        fetch('/Support/SendSupportRequest', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                userName,
                userEmail,
                userMessage
            }),
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                showMassage();
                setTimeout(hideMassage, 2000);
                return response.json();
            })
            .catch(error => {
                showErrorMassage();
                setTimeout(hideErrorMassage, 2000);
            })
    }
});