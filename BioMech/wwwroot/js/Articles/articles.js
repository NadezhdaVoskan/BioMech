var editor = new FroalaEditor('#example');
let selectedFile = null;

function loadArticlesByCategory(categoryId) {
    // Отправка запроса на сервер для получения статей по выбранной категории
    fetch(`/Articles/GetArticlesByCategory?categoryId=${categoryId}`)
        .then(response => response.json())
        .then(data => {
            const articlesContainer = document.querySelector('.articles');
            articlesContainer.innerHTML = ''; // Очистка содержимое контейнера статей
            data.forEach(article => {
                // HTML для каждой статьи и добавление его в контейнер статей
                articlesContainer.innerHTML += `
                    <div class="article">
                        <div class="article__content">
                            <div class="article__img">
                                <img src="${article.image_Article}" alt="">
                            </div>
                            <div class="article__description">
                                <h3>${article.name_Article}</h3>
                                <p class="article__text">${article.shortDescriptionArticle}</p>
                                <form action="/Articles/Article" method="get" target="_blank" class="article-form">
                                    <input type="hidden" name="idArticle" value="${article.idArticle}" />
                                    <button type="submit" class="more">Читать<img src="../img/icon.png" alt=""></button>
                                </form>
                            </div>
                        </div>
                        <p class="article__date">Опубликовано: ${new Date(article.publicationDateArticle).toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' })}</p>
                    </div>
                `;
            });
        })
        .catch(error => {
            console.error('Ошибка при загрузке статей:', error);
        });
}


document.getElementById('articleForm').addEventListener('submit', function (e) {
    e.preventDefault();

    // HTML из редактора
    var htmlContent = editor.html.get();

    // Метка времени
    var timestamp = new Date().getTime();

    // Создание файла с уникальным именем
    var blob = new Blob([htmlContent], { type: 'text/html' });
    var fileName = "articleContent_" + timestamp + ".html";
    var file = new File([blob], fileName, { type: "text/html" });

    var formData = new FormData(this);

    formData.append("htmlFile", file);

    if (selectedFile) {
        formData.append("articlePhoto", selectedFile); 
    }

    fetch(this.action, {
        method: 'POST',
        body: formData
    }).then(response => {
        if (response.ok) {
            window.location.href = "/Articles/Articles"
            return response.json();
        } else {
            throw new Error('Проблема с сохранением статьи');
        }
    }).catch(error => {
        console.error('Ошибка:', error);
    });
});


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

//Функция для загрузки фотографии
function upload(selector, options = {}) {
    let files = []
    const onUpload = options.onUpload ?? noop

    const confirmModal = document.getElementById('confirmModal');
    const yesBtn = document.getElementById('yesBtn');
    const noBtn = document.getElementById('noBtn');
    const photo__ava = document.getElementById('photo__ava');
    const photoProfile = document.getElementById('photoProfile');
    const input = document.querySelector(selector)
    const open = element('button', ['PD__button', 'btn'], 'Открыть')
    const upload = element('button', ['PD__button', 'btn'], 'Сохранить')
    const del = element('button', ['PD__button', 'btn'], 'Удалить')
    upload.style.display = 'none'

    if (options.accept && Array.isArray(options.accept)) {
        input.setAttribute('accept', options.accept.join(','))
    }

    open.type = 'button';
    upload.type = 'button';
    del.type = 'button';

    input.insertAdjacentElement('afterend', del)
    input.insertAdjacentElement('afterend', upload)
    input.insertAdjacentElement('afterend', open)

    if (photoProfile != null) {
        del.style.display = 'none'
    } else {
        del.style.display = 'block'
    }

    const triggerInput = () => input.click()

    // При нажатии "Открыть" сохраняет и размещает фотографию
    const changeHandler = event => {
        if (!event.target.files.length) {
            return;
        }

        files = Array.from(event.target.files);
        selectedFile = files[0]; 
        photo__ava.innerHTML = '';
        upload.style.display = 'block';

        const reader = new FileReader();

        reader.onload = ev => {
            const src = ev.target.result;
            photo__ava.insertAdjacentHTML('afterbegin', `
        <div class="photo__ava__remove">
            <div class="remove" data-name="${selectedFile.name}">&times;</div>
            <img src="${src}" alt="${selectedFile.name}"/>
        </div>
        `);
        };

        reader.readAsDataURL(selectedFile);
        del.style.display = 'none';
    };


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

    // Выполняет функцию кнопки "Удалить фотографию"
    const deleteHandler = () => {
        showModal() // Выводит окно

        // Выполняет действия при нажатии на кнопку "ДА"
        yesBtn.addEventListener('click', () => {
            // Запрос на удаление фотографии
            fetch('/Articles/RemovePhotoArticle', {
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

    // Выполняет функцию кнопки "Сохранить"
    const uploadHandler = () => {
        const formData = new FormData();
        files.forEach(file => {
            formData.append('userPhoto', file);
        });

        photo__ava.querySelectorAll('.remove').forEach(e => e.remove())
        onUpload(files)
        del.style.display = 'block'
        upload.style.display = 'none'
    }

    open.addEventListener('click', triggerInput)
    input.addEventListener('change', changeHandler)
    photo__ava.addEventListener('click', removeHandler)
    upload.addEventListener('click', uploadHandler)
    del.addEventListener('click', deleteHandler)
}

upload('#file', {
    accept: ['.png', '.jpg', '.jpeg', '.gif'],
    onUpload(files) {
        console.log('Files:', files)
    }
})

//Функция удаления статьи
function submitDeleteForm(articleId) {
        document.getElementById('deleteArticleForm-' + articleId).submit();
}

//Функция публикации статьи
function submitPublicForm(articleId) {
    document.getElementById('publicArticleForm-' + articleId).submit();
}

//Функция архивации статьи
function archiveArticle() {
    var form = document.getElementById('articleForm');
    var isEditing = form.querySelector('[name="isEditing"]').value.toLowerCase() === 'true';
    var actionUrl = isEditing ? '/Articles/ArchiveArticle' : '/Articles/ArchiveNewArticle'; 

    if (!form) {
        console.error('Form not found');
        return;
    }

    var formData = new FormData(form);

    var htmlContent = editor.html.get();
    var blob = new Blob([htmlContent], { type: 'text/html' });
    var timestamp = new Date().getTime();
    var fileName = "articleContent_" + timestamp + ".html";
    var file = new File([blob], fileName, { type: "text/html" });

    formData.append("htmlFile", file);

    if (selectedFile) {
        formData.append("articlePhoto", selectedFile);
    }

    fetch(actionUrl, {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (response.ok) {
                window.location.href = "/Articles/Articles";
            } else {
                return response.text().then(text => { throw new Error(text); });
            }
        })
        .catch(error => {
            console.error('Ошибка:', error);
        });
}


document.addEventListener('DOMContentLoaded', function () {
    const editor = new FroalaEditor('#example');
    const contentUrlField = document.getElementById('contentUrl');
    const contentUrl = contentUrlField.value;

    fetch(`/Proxy/GetContent?url=${encodeURIComponent(contentUrl)}`)
        .then(response => response.text())
        .then(htmlContent => {
            editor.html.set(htmlContent);
        })
        .catch(error => {
            console.error('Ошибка при загрузке HTML:', error);
            editor.html.set('<p>Ошибка при загрузке содержимого. Пожалуйста, проверьте ссылку и попробуйте снова.</p>');
        });
});
