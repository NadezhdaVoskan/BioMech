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

function noop() {

}

function upload(selector, options = {}) {
    let files = []
    const onUpload = options.onUpload ?? noop

    const confirmMassage = document.getElementById('confirmMassage');
    const confirmErrorMassage = document.getElementById('confirmErrorMassage');
    const confirmModal = document.getElementById('confirmModal');
    const yesBtn = document.getElementById('yesBtn');
    const noBtn = document.getElementById('noBtn');
    const photo__ava = document.getElementById('photo__ava');
    const changePassword = document.getElementById('changePassword');
    const changeBlock = document.getElementById('changeBlock');
    const photoProfile = document.getElementById('photoProfile');
    const input = document.querySelector(selector)
    const open = element('button', ['PD__button', 'btn'], 'Открыть')
    const upload = element('button', ['PD__button', 'btn'], 'Сохранить')
    const del = element('button', ['PD__button', 'btn'], 'Удалить')
    upload.style.display = 'none'

    if (options.accept && Array.isArray(options.accept)) {
        input.setAttribute('accept', options.accept.join(','))
    }

    input.insertAdjacentElement('afterend', del)
    input.insertAdjacentElement('afterend', upload)
    input.insertAdjacentElement('afterend', open)

    console.log(files)

    if (photoProfile != null) {
        del.style.display = 'none'
    } else {
        del.style.display = 'block'
    }

    const triggerInput = () => input.click()

    // При нажатии "Открыть" сохраняет и размещает фотографию
    const changeHandler = event => {
        if (!event.target.files.length) {
            return
        }

        files = Array.from(event.target.files)
        photo__ava.innerHTML = ''
        upload.style.display = 'block'

        files.forEach(file => {
            if (!file.type.match('image')) {
                return
            }

            const reader = new FileReader()

            reader.onload = ev => {
                const src = ev.target.result
                photo__ava.insertAdjacentHTML('afterbegin', `
                <div class="photo__ava__remove">
                    <div class="remove" data-name="${file.name}">&times;</div>
                    <img src="${src}" alt="${file.name}"/>
                </div>
                `)
            }

            reader.readAsDataURL(file)
        })
        del.style.display = 'none'
    }

    // Удаляет фотографию
    const removeHandler = event => {

        if (!event.target.dataset.name) {
            return
        }

        const { name } = event.target.dataset
        files = files.filter(file => file.name !== name)

        if (!files.length) {
            upload.style.display = 'none'
        }

        const block = photo__ava.querySelector(`[data-name="${name}"]`).closest('.photo__ava__remove')
        block.remove()
        if (!files.length) {
            del.style.display = 'none'
        }

    }

    // Изменение почты
    const emailForm = document.getElementById('update-email-form');
    if (emailForm) {
        emailForm.addEventListener('submit', function (event) {

            if (validation(this) == true) {
                showMassage();
                setTimeout(hideMassage, 2000);
                event.preventDefault(); // Предотвратить стандартную отправку формы
                var email = document.querySelector('input[name="email"]').value; // Получить значение из поля ввода

                fetch('/PersonalData/UpdateEmail', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                    },
                    body: `email=${encodeURIComponent(email)}`
                })
                    .then(response => {
                        if (response.ok) {
                        
                        } else {
                            throw new Error('Не удалось обновить почту');
                        }
                    })
                    .catch(error => {
                        alert("Ошибка при обновлении почты: " + error.message);
                    });
            } else {
                event.preventDefault()
            }
        });
    }

    // Изменение имени и фамилии
    const nameForm = document.getElementById('update-name-form');
    if (nameForm) {
        nameForm.addEventListener('submit', function (event) {

            if (validation(this) === true) {
                showMassage();
                setTimeout(hideMassage, 2000);
                event.preventDefault(); // Предотвратить стандартную отправку формы
                var firstName = document.querySelector('input[name="firstname"]').value; // Получить значение из поля ввода для имени
                var surname = document.querySelector('input[name="surname"]').value; // Получить значение из поля ввода для фамилии

                fetch('/PersonalData/UpdateName', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                    },
                    body: `firstname=${encodeURIComponent(firstName)}&surname=${encodeURIComponent(surname)}`
                })
                    .catch(error => {
                        alert("Ошибка при обновлении личных данных: " + error.message);
                    });
            } else {
                event.preventDefault()
            }
        });
    }

    // Выполняет функцию кнопки "Удалить фотографию"
    const deleteHandler = () => {
        showModal() // Выводит окно

        // Выполняет действия при нажатии на кнопку "ДА"
        yesBtn.addEventListener('click', () => {
            // Запрос на удаление фотографии профиля
            fetch('/PersonalData/RemovePhotoProfile', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                }

            })
                .then(response => {
                    if (response.ok) {
                        window.location.reload();
                        setTimeout(hideMassage, 1000);
                    } else {
                        alert('Произошла ошибка при удалении фотографии.');
                    }
                })
                .catch(error => {
                    console.error('Ошибка:', error);
                });
            hideModal(); /* Код для "Да" */
        });

        // Выполняет действия при нажатии на кнопку "НЕТ"
        noBtn.addEventListener('click', () => { hideModal(); /* Код для "Нет" */ });

    }

    function showModal() { confirmModal.style.display = 'block'; } // Показывает окно
    function hideModal() { confirmModal.style.display = 'none'; } // Скрывает окно
    function showMassage() { confirmMassage.style.display = 'block'; }// Показывает сообщение
    function hideMassage() { confirmMassage.style.display = 'none'; }
    function showErrorMassage() { confirmErrorMassage.style.display = 'block'; }// Показывает сообщение
    function hideErrorMassage() { confirmErrorMassage.style.display = 'none'; }
    // Выполняет функцию кнопки "Сохранить"
    const uploadHandler = () => {
        const formData = new FormData();
        files.forEach(file => {
            formData.append('userPhoto', file);
        });

        fetch('/PersonalData/UploadPhotoProfile', {
            method: 'POST',
            body: formData,
        })
            .then(response => {
                if (response.ok) {
                    window.location.href = '/PersonalAccount'
                }
                throw new Error('Network response was not ok.');
            })
            .then(data => {
                console.log(data);
            })
            .catch(error => {
                console.error('problem:', error);
            });

        photo__ava.querySelectorAll('.remove').forEach(e => e.remove())
        onUpload(files)
        del.style.display = 'block'
        upload.style.display = 'none'
    }



    // Выполняет функцию кнопки "Изменить пароль" на странице личного кабинета
    const changePasswordHandler = () => {
        changeBlock.insertAdjacentHTML('afterend', `
             <div class="autorization-field emergence">
                 <input data-password="true" id="passwordThree" name="oldPassword" placeholder="Введите старый пароль">
             </div>
             <div class="autorization-field emergence">
                 <input data-password="true" data-check-new-old-one="true" id="passwordOne" name="newPassword" placeholder="Введите новый пароль">
             </div>
             <div class="autorization-field">
                 <input data-password="true" data-check-new-old="true" data-repeat-password="true" id="passwordTwo" name="repeatedNewPassword" placeholder="Подтвердите новый пароль">
             </div>
                `)
        changeBlock.remove()
        changePassword.insertAdjacentHTML('afterend', `
             <button type="submit" class="PD__button" id="confirmChangePassword">Подтвердить пароль</button>
             `)
        changePassword.remove()

        document.getElementById('confirmChangePassword').addEventListener('click', function (event) {

            const oldPassword = document.querySelector('input[name="oldPassword"]').value;
            const newPassword = document.querySelector('input[name="newPassword"]').value;
            const repeatedNewPassword = document.querySelector('input[name="repeatedNewPassword"]').value;
            if (validation(document.getElementById('add-form'))) {
                fetch('/PersonalData/UpdatePassword', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        oldPassword,
                        newPassword,
                        repeatedNewPassword
                    }),
                })
                    .then(response => {
                        if (!response.ok) {
                            throw new Error('Network response was not ok');
                        }
                        return response.json();
                    })
                    .catch(error => {
                        showErrorMassage();
                        setTimeout(hideErrorMassage, 2000);
                    })
            } else {
                event.preventDefault()
            }
        });
    }

    open.addEventListener('click', triggerInput)
    input.addEventListener('change', changeHandler)
    photo__ava.addEventListener('click', removeHandler)
    upload.addEventListener('click', uploadHandler)
    del.addEventListener('click', deleteHandler)
    changePassword.addEventListener('click', changePasswordHandler)
}

upload('#file', {
    accept: ['.png', '.jpg', '.jpeg', '.gif'],
    onUpload(files) {
        console.log('Files:', files)
    }
})
