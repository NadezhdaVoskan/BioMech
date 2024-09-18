function showMassage() { confirmMassage.style.display = 'block'; }
function hideMassage() { confirmMassage.style.display = 'none'; }
function validation(form) {

    //Удаление статуса ошибки
    function removeError(input) {
        const parent = input.parentNode;

        if (parent.classList.contains('error')) {
            parent.querySelector('.error-label').remove()
            parent.classList.remove('error')
        }
    }
    function removeErrorTextarea(textarea) {
        const textParent = textarea.parentNode;

        if (textParent.classList.contains('error')) {
            textParent.querySelector('.error-label').remove()
            textParent.classList.remove('error')
        }
    }

    //Создание статуса ошибки
    function createError(input, text) {
        const parent = input.parentNode;
        const errorLabel = document.createElement('label')

        errorLabel.classList.add('error-label')
        errorLabel.textContent = text

        parent.classList.add('error')

        parent.append(errorLabel)
    }

    function createErrorTextarea(textarea, text) {
        const textParent = textarea.parentNode;
        const errorLabel = document.createElement('label')

        errorLabel.classList.add('error-label')
        errorLabel.textContent = text

        textParent.classList.add('error')

        textParent.append(errorLabel)
    }


    let result = true;

    const passwordOne = document.getElementById('passwordOne');
    const passwordTwo = document.getElementById('passwordTwo');
    const passwordThree = document.getElementById('passwordThree');
    const allInputs = form.querySelectorAll('input');
    const allTextarea = form.querySelectorAll('textarea');
    const pass = /(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,}/;
    const re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

    //Вывод текста ошибки при разных условиях
    for (const input of allInputs) {

        removeError(input)

        if (input.dataset.checkOld) {
            if (passwordTwo.value != 'К') {
                removeError(input)
                createError(input, 'Вы ввели не старый пароль!')
                result = false
            }
        }

        if (input.dataset.repeatPassword) {
            if (passwordOne.value != passwordTwo.value) {
                removeError(input)
                createError(input, 'Новые пароли не совпадают!')
                result = false
            }
        }

        if (input.dataset.checkNewOld) {
            if (passwordTwo.value == passwordThree.value) {
                removeError(input)
                createError(input, 'Новый пароль совпадает со старым!')
                result = false
            }
        }

        if (input.dataset.checkNewOldOne) {
            if (passwordOne.value == passwordThree.value) {
                removeError(input)
                createError(input, 'Новый пароль совпадает со старым!')
                result = false
            }
        }

        if (input.dataset.password) {
            if (!pass.test(input.value)) {
                removeError(input)
                createError(input, `Пример пароля: Pa$$w0rd123`)
                result = false
            }
        }

        if (input.dataset.email) {
            if (!re.test(input.value)) {
                removeError(input)
                createError(input, `Пример эл.почты: examle@ya.com`)
                result = false
            }
        }

        if (input.dataset.minLength) {
            if (input.value.length < input.dataset.minLength) {
                removeError(input)
                createError(input, `Минимальное кол-во символов: ${input.dataset.minLength}`)
                result = false
            }
        }

        if (input.dataset.maxLength) {
            if (input.value.length > input.dataset.maxLength) {
                removeError(input)
                createError(input, `Максимальное кол-во символов: ${input.dataset.maxLength}`)
                result = false
            }
        }

        if (input.dataset.required == "true") {
            if (input.value == "") {
                removeError(input)
                createError(input, 'Поле не заполнено!')
                result = false
            }
        }

    }
    for (const textarea of allTextarea) {

        removeErrorTextarea(textarea)

        if (textarea.dataset.minLength) {
            if (textarea.value.length < textarea.dataset.minLength) {
                removeErrorTextarea(textarea)
                createErrorTextarea(textarea, `Минимальное кол-во символов: ${textarea.dataset.minLength}`)
                result = false
            }
        }

        if (textarea.dataset.maxLength) {
            if (textarea.value.length > textarea.dataset.maxLength) {
                removeErrorTextarea(textarea)
                createErrorTextarea(textarea, `Максимальное кол-во символов: ${textarea.dataset.maxLength}`)
                result = false
            }
        }

        if (textarea.dataset.required == "true") {
            if (textarea.value == "") {
                removeErrorTextarea(textarea)
                createErrorTextarea(textarea, 'Поле не заполнено!')
                result = false
            }
        }

    }

    return result
}
const elementsList = document.querySelectorAll("#add-form");
const elementsArray = [...elementsList];

// Ищет айди кнопки, по которой был сделан клик и выводит сообщение
elementsArray.forEach(element => {
    const form = element;

    form.addEventListener('submit', function (event) {
        

        if (validation(this) === true) {
            showMassage();
            setTimeout(hideMassage, 2000);
        } else {
            event.preventDefault()
        }
    })
})
//Проверка пароля на совпадение
function show_hide_password(target) {
    let input = document.getElementById('passwordOne');
    if (input.getAttribute('type') == 'password') {
        target.classList.add('view');
        input.setAttribute('type', 'text');
    } else {
        target.classList.remove('view');
        input.setAttribute('type', 'password');
    }
    return false;
}
function hide_password(target) {
    let input = document.getElementById('passwordTwo');
    if (input.getAttribute('type') == 'password') {
        target.classList.add('view');
        input.setAttribute('type', 'text');
    } else {
        target.classList.remove('view');
        input.setAttribute('type', 'password');
    }
    return false;
}