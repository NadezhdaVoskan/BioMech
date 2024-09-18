const confirmModal = document.getElementById('confirmModal');
const yesBtn = document.getElementById('yesBtn');
const noBtn = document.getElementById('noBtn');

document.querySelector('#confirmModal p').textContent = 'Вы уверены, что хотите удалить данную категорию?';
function showModal() { confirmModal.style.display = 'block'; }
function hideModal() { confirmModal.style.display = 'none'; }

//Выводит окно с вопросам, удалить категорию или нет
$('#deleteCategoryButton').click(function (ev) {
    ev.preventDefault();
    showModal() 
    yesBtn.addEventListener('click', () => {
        $(this).unbind('click').click()
        hideModal();
    });
    noBtn.addEventListener('click', () => { hideModal(); ev.preventDefault(); });

});
