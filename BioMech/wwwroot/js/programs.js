const mark = document.getElementById('mark');
const width = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
console.log(width)
let slidesQuantity;
if (width > 931) {
    slidesQuantity = 7;
} else {
    slidesQuantity = 4;
}
//Загрузка категорий программ
function loadCategoryPrograms(button) {
    const idCategory = button.getAttribute('data-category-id');
    window.location.href = `/TrainingPrograms/Program?categoryId=${idCategory}`;
}

//Настройка слайдера дней
new Swiper('.swiper', {
    navigation: {
        nextEl: '.slide-next',
        prevEl: '.slide-prev'
    },
    pagination: {
        el: '.swiper-pagination',
        clickable: true,
        dynamicBullets: true
    },
    hashNavigation: {
        watchState: true,
    },
    keyboard: {
        enabled: true,
        onlyInViewport: true
    },

    slidesPerView: slidesQuantity,

});

