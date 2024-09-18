//Функция проверки на наличие обложки видеоролика и добавление заглушки
document.addEventListener("DOMContentLoaded", function () {
    var imageUrlInput = document.querySelector('input[name="imageVideoTraining"]');
    var imageDisplay = document.getElementById('imageTrainingVideo');

    imageUrlInput.addEventListener('input', function () {
        imageDisplay.src = imageUrlInput.value || '~/img/articles/articles.svg';
    });


});

function submitPublicForm(trainingId) {
    document.getElementById('publicTrainingForm-' + trainingId).submit();
}
