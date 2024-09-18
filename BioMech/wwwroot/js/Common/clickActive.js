const ind = document.getElementById('ind');
const pro = document.getElementById('pro');
const aut = document.getElementById('aut');
const art = document.getElementById('art');
const per = document.getElementById('per');
const lic = document.getElementById('lic');
const uda = document.getElementById('uda');
const dia = document.getElementById('dia');

//Проверка на какой странице находится пользователь, добавляя ссылке статус "Активная"
let pathname = window.location.pathname;

let url;


let modelType;

// Определение страницы для диагностики
switch (pathname) {
    case "/diagnostics/DiagnosticsBlade":
        url = '/Diagnostics/UploadPhotoForModel'
        modelType = 'ShoulderBlades';
        dia.classList.add('active')
        break;
    case "/diagnostics/DiagnosticsFeet":
        url = '/Diagnostics/UploadPhotoForModel'
        modelType = 'FootBone';
        dia.classList.add('active')
        break;
    case "/diagnostics/DiagnosticsKnees":
        url = '/Diagnostics/UploadPhotoForModel'
        modelType = 'KneesProblems'
        dia.classList.add('active')
        break;
    case "/diagnostics/DiagnosticsBody":
        dia.classList.add('active')
        break;
    case "/diagnostics/DiagnosticsNeck":
        url = '/Diagnostics/UploadPhotoForModel'
        modelType = 'NeckProtraction'
        dia.classList.add('active')
        break;
}

switch (pathname) {
    case "/":
        ind.classList.add('active')
        break;
    case "/articles/Article":
    case "/articles/Articles":
    case "/Articles/SearchArticle":
    case "/articles/CreateArticles":
    case "/articles/Archive":
    case "/articles/CreateCategories":
        art.classList.add('active')
        break;
    case "/trainingPrograms/Program":
    case "/TrainingPrograms/Program":
    case "/trainingPrograms/Programs":
    case "/TrainingPrograms/ProgramVideo":
    case "/trainingPrograms/CreatePrograms":
    case "/trainingPrograms/ArchivePrograms":
    case "/trainingPrograms/TrainingPrograms":
    case "/TrainingPrograms/ViewingTrainingPrograms":
    case "/TrainingPrograms/EditingPrograms":
        pro.classList.add('active')
        break;
    case "/diagnostics/DiagnosticsDescription":
        dia.classList.add('active')
        break;
    case "/authentication/Authorization":
    case "/authentication/Registration":
    case "/authentication/Forgot":
    case "/authentication/Recovery":
        aut.classList.add('active')
        break;
    case "/PersonalData/ProfileResults":
    case "/PersonalData/Profile":
        per.classList.add('active')
        break;
    case "/PersonalData/PersonalAccount":
        per.style.display = 'block'
        lic.classList.add('active__acc')
        per.classList.add('active')
        break;
    case "/PersonalData/DeletePersonalAccount":
        per.style.display = 'block'
        uda.classList.add('active__acc')
        per.classList.add('active')
        break;
}

//Появление списка категорий
        function myFunction() {
    document.getElementById("myDropdown").classList.toggle("show");
}

window.onclick = function(event) {
    if (!event.target.matches('.сategories')) {

    var dropdowns = document.getElementsByClassName("dropdown-content");
    var i;
    for (i = 0; i < dropdowns.length; i++) {
      var openDropdown = dropdowns[i];
      if (openDropdown.classList.contains('show')) {
        openDropdown.classList.remove('show');
      }
    }
  }
}
const headerMenu = document.getElementById('headerMenu')
const headerHamburger = document.getElementById('headerHamburger')
const headerArticleSubmenu = document.getElementById('headerArticleSubmenu')
const headerProgramSubmenu = document.getElementById('headerProgramSubmenu')
const headerDiagnosticsSubmenu = document.getElementById('headerDiagnosticsSubmenu')

//Обработка клика по иконке "Гамбургер" в шапке и появляение боковой навигации для телефонной версии сайта
headerHamburger.addEventListener('click', function () {
    if (headerMenu.classList.contains('submenu-active')) {
        headerMenu.classList.remove('submenu-active')
        headerHamburger.classList.remove('hamburger-opacity')
    } else {
        headerMenu.classList.add('submenu-active')
        headerHamburger.classList.add('hamburger-opacity')
    }
});

//Появление списка "Статьи" для редактора
headerArticleSubmenu.addEventListener('click', function () {
    if (headerArticleSubmenu.classList.contains('button-active-sub')) {
        headerArticleSubmenu.classList.remove('button-active-sub')
        headerProgramSubmenu.classList.remove('header-program')
    } else {
        headerArticleSubmenu.classList.add('button-active-sub')
        headerProgramSubmenu.classList.add('header-program')
    }
});

//Появление списка "Программы" для редактора
headerProgramSubmenu.addEventListener('click', function () {
    if (headerProgramSubmenu.classList.contains('button-active-sub')) {
        headerProgramSubmenu.classList.remove('button-active-sub')
        headerDiagnosticsSubmenu.classList.remove('header-diagnostics')
    } else {
        headerProgramSubmenu.classList.add('button-active-sub')
        headerDiagnosticsSubmenu.classList.add('header-diagnostics')
    }
});
