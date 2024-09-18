document.addEventListener('DOMContentLoaded', function () {
    const confirmModal = document.getElementById('confirmModal');
    const yesBtn = document.getElementById('yesBtn');
    const noBtn = document.getElementById('noBtn');
    function showModal() { confirmModal.style.display = 'block'; }
    function hideModal() { confirmModal.style.display = 'none'; }

    const resultsDiagnostics = document.querySelector('.results__diagnostics');

    //Реагирование клика на крестик при наведении на фотографию
    resultsDiagnostics.addEventListener('click', function (event) {

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

    // Функция удаления фотографии
    async function removePhoto(photoId) {
        try {
            const response = await fetch(`/PersonalData/DeletePhotoDiagnostic?photoDiagnosticsId=${photoId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            // Удаление блока с фотографией из DOM
            const resultBlock = document.querySelector(`.result[data-photo-id="${photoId}"]`);
            resultBlock.parentNode.removeChild(resultBlock);
        } catch (error) {
            console.error(error);
        }
    }

    //Очистка истории диагностик
    const clearDiagnosticHistoryBtn = document.getElementById('clearDiagnosticHistoryBtn');
    if (clearDiagnosticHistoryBtn) {
        clearDiagnosticHistoryBtn.addEventListener('click', async function () {
            try {
                const urlParams = new URLSearchParams(window.location.search);
                const categoryId = urlParams.get('categoryId');
                if (!categoryId) {
                    console.error('categoryId не найден в URL');
                    return;
                }
                const response = await fetch(`/PersonalData/DeletePhotoDiagnosticsByCategory?categoryId=${categoryId}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });
                // Перенаправление на страницу с профилем после успешного удаления
                window.location.href = '/PersonalData/ProfileResults';
            } catch (error) {
                console.error(error);
            }
        });
    }
});

const resultPhotoBig = document.getElementById('resultPhotoBig');
const closeImgBig = document.getElementById('closeImgBig');
const bigImg = document.getElementById('bigImg');

//Увеличие фотографии при клике
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
