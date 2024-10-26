# BioMech
WEB-приложения для анализа и коррекции биомеханики движения при помощи нейросетевых технологий «BioMech».

## Роли пользователей
В WEB-приложении представлено 3 роли пользователей:
* Неавторизованный пользователь: имеет возможность просматривать статьи, тренировочные программы, написать письмо в поддержку, восстановить пароль. 
* Обычный авторизованный пользователь: имеет возможность проходить диагностики, тренировочные программы, где они могут отмечать выполненные дни, а также досрочно заканчивать выполнение программы, обновить свои данные или удалить аккаунт, доступна история диагностики, переход на текущую тренировку, управление журналом тренировок и отслеживание прогресса по оценкам.
* Редактор: имеет возможность управлять статьями и тренировочными программами.

## Функциональность
* Учет проверенных фотографий на патологии пользователей, выполнения тренировочных программ;
* Просмотр данных о статьях, тренировочных программ, проверенных фотографий на патологии, личных данных вошедшего пользователя, журнала тренировок, графиков оценивания;
* Поиск и фильтрация данных в статьях и тренировочных программах;
* Авторизация, регистрация, восстановление пароля, выход из аккаунта, удаление аккаунта;
* Отправление сообщения на почту поддержки;
* Редактирование статей, тренировочных программ.

## Архитектура приложения
В WEB-приложении используются две API. Первая API используется для взаимодействия с базой данных Microsoft SQL Server, на котором хранятся данные, и для получения и отправки данных. Вторая API используется для взаимодействия с нейросетевыми технологиями.

<div align="center">
  <img src="https://github.com/user-attachments/assets/52461f0d-fee1-427b-b441-4cc9717cae4e" alt="Архитектура приложения">
</div>

## Модели
### 1. Выявление крыловидных лопаток
Обучение было выполнено с помощью модели YOLOv8.

<div align="center">
  <img src="https://github.com/user-attachments/assets/1b37cf6e-0f07-40e4-901d-cb8866cc7909" alt="Выявление крыловидных лопаток">
</div>

### 2. Выявление наличия косточки на ноге
Обучение было выполнено с помощью модели YOLOv8.

<div align="center">
  <img src="https://github.com/user-attachments/assets/070e048f-3c8f-4c7c-b4e3-d13c8dfd8d4a" alt="Выявление косточки на ноге" width="50%">
</div>

### 3. Выявление протракции шеи
Была использована модель детектирования позы человека при помощи библиотеки OpenPose от OpenCV.

<div align="center">
  <img src="https://github.com/user-attachments/assets/29d12b7a-4fb4-470b-a9cb-a986d334b5df" alt="Протракция шеи" width="50%">
</div>

Определение протракции шеи строится математически. Строится линия между точкой уха и точкой на плече программно, далее на уровне уха строится вертикальная линия, после чего вычисляется угол.  
Для этого написаны 2 программы: 
1. Выводит все точки, которые обнаружила модель (их в сумме 25). Для того, чтобы проверить, что модель вычисляет правильно нужные мне точки.
2. Выводится угол, где я проверяла на нескольких фотографиях их зависимость, ориентируясь на свой взгляд на фотографии.  
В итоге проверки я вычислила, что примерно ниже 7 градусов нормальная шея, а выше уже "компьютерная шея".

<div align="center">
  <img src="https://github.com/user-attachments/assets/4ccd3a31-949b-4b61-ab4a-3628a2ee4674" alt="Протракция шеи 2">
</div>

Точки, которые строятся с помощью OpenPose: 

<div align="center">
  <img src="https://github.com/user-attachments/assets/ead580fe-dc4e-40a7-a961-df8cf2b735d6" alt="Протракция шеи 3" width="30%">
</div>

### 4. Выявление вальгуса или варуса колен
Была использована модель детектирования позы человека при помощи библиотеки OpenPose от OpenCV.

<div align="center">
  <img src="https://github.com/user-attachments/assets/63e81e68-ff79-4990-b442-00cf2d359e83" alt="Вальгус и варус колен" width="30%">
</div>

Определение вальгуса и варуса строится математически. Модель определяет точки, далее рисуется линия между 3 точками (9, 10, 11 из картинки выше).

Получается условие:
- Вальгус <174
- Норма 174 < angle < 182
- Варус >182

Это примерно как на последнем изображении, только там другие градусы, так как взяты другие точки.  
Здесь также проведен анализ на фотографиях с 3 классами, которые перечислены выше.

<div align="center">
  <img src="https://github.com/user-attachments/assets/37bc5f43-4177-47a3-adb8-2e077b8257a9" alt="Анализ Вальгуса и Варуса" >
</div>

## Примеры страниц, функциональность веб-приложения
### Понятие термина "Биомеханики

<div align="center">
  <img src="https://github.com/user-attachments/assets/23b2381f-28e4-47fa-811e-cace114c620b" alt="Понятие термина Биомеханика" width="70%">
</div>

### Предлагаемые возможности

<div align="center">
  <img src="https://github.com/user-attachments/assets/684b5673-8c00-4e06-b2ef-e923e2d45209" alt="Предлагаемые возможности" width="70%">
</div>

### Предложение пройти диагностику

<div align="center">
  <img src="https://github.com/user-attachments/assets/f77f4d92-80d8-4783-88a2-e2d43157dbf2" alt="Предложение пройти диагностику" width="70%">
</div>

### Отзывы о программе

<div align="center">
  <img src="https://github.com/user-attachments/assets/e1c57106-2e80-4ba8-a7e1-7ce95ff7717d" alt="Отзывы о программе" width="70%">
</div>

### Статьи

<div align="center">
  <img src="https://github.com/user-attachments/assets/1681a7ae-7392-41e7-9cd0-4fa0ccc1aa74" alt="Статьи" width="70%">
</div>

### Пример программ

<div align="center">
  <img src="https://github.com/user-attachments/assets/a2062665-4b76-40fd-bed9-7e7a8232a555" alt="Пример программ 1" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/5ea4cf57-a9b5-42b9-9bd6-76aee7009eb1" alt="Пример программ 2" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/1a906fc5-420d-4693-bea7-0574d4d0a111" alt="Пример программ 3" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/5c008425-b9d5-4a89-8a10-2b6f7eb1bdcb" alt="Пример программ 4" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/0815a1f3-5119-4c5d-b434-6d3ae712e838" alt="Пример программ 5" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/4204ac03-8222-49d5-884b-2b06f73ba3d6" alt="Пример программ 6" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/fb95ffa0-b12d-456c-a81e-961da9f84e24" alt="Пример программ 7" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/0c756e63-50c5-4604-abc3-100a211211c9" alt="Пример программ 8" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/132d6455-ca45-486b-a644-8efdeb7983ce" alt="Пример программ 9" width="70%">
</div>

### Страница диагностики

<div align="center">
  <img src="https://github.com/user-attachments/assets/da3db8d7-e44f-4eeb-998c-c23f6c190923" alt="Страница диагностики 1" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/ee901cc2-3260-46d8-97ae-adfe234a73d1" alt="Страница диагностики 2" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/e4f841ec-bf87-459c-906f-9c887942726c" alt="Страница диагностики 3" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/bf16f3b1-60eb-43cd-a1a1-f33e51882c24" alt="Страница диагностики 4" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/bf72cfaa-121b-40ce-bc86-4fe573c15f4a" alt="Страница диагностики 5" width="70%">
</div>

### Пример одной из страниц диагностики

<div align="center">
  <img src="https://github.com/user-attachments/assets/ec6f885a-cff5-4766-8bda-1ca57cda1053" alt="Пример страницы диагностики 1" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/4a78b557-5341-4108-b85e-1023a7a375d8" alt="Пример страницы диагностики 2" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/b7c06c8d-0236-4de0-8142-f42a8ebde678" alt="Пример страницы диагностики 3" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/8d6e85e4-29d4-407a-b7f8-7146075d692c" alt="Пример страницы диагностики 4" width="70%">
</div>

### Авторизация, регистрация, восстановление пароля, поддержка

<div align="center">
  <img src="https://github.com/user-attachments/assets/b5d90edf-415c-4026-ae6b-9760248f0ba8" alt="Авторизация 1" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/290abd12-fd5c-4358-a9d2-66250e0c0e39" alt="Авторизация 2" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/6636b70c-137d-4bbb-85c7-ddb6f7e68724" alt="Авторизация 3" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/fe5e6e6d-60f7-4018-9773-4e52314d85ec" alt="Авторизация 4" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/21d9c1e9-5482-455a-9c47-31b1ecb9af8b" alt="Авторизация 5" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/b80e7969-fecf-4dc4-978b-b9478488fe3e" alt="Авторизация 6" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/d2545056-4805-4ab2-9707-23c24c91c8e6" alt="Авторизация 7" width="70%">
</div>

### Личный кабинет

<div align="center">
  <img src="https://github.com/user-attachments/assets/323cab26-5667-4bcf-8528-ee9974d0f036" alt="Личный кабинет 1" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/46e89db4-6ca4-427c-a0fe-1fd669be0029" alt="Личный кабинет 2" width="70%">
</div>

### Профиль

<div align="center">
  <img src="https://github.com/user-attachments/assets/d39f206b-1b3f-47da-8250-c8ec9abf347c" alt="Профиль 1" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/160e3a26-6f00-4e2e-8e9e-d960b7f637c2" alt="Профиль 2" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/60f4b5fb-b8b2-44f6-8b98-ba79bf4cad20" alt="Профиль 3" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/6d2c40cb-d861-4760-a7ee-94cc527dc6a8" alt="Профиль 4" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/9a55f07e-52cf-4f60-9cc5-12d772706c4d" alt="Профиль 5" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/198b6daf-a739-4d40-831f-7964eabec720" alt="Профиль 6" width="70%">
</div>

### Тренировочная программа

<div align="center">
  <img src="https://github.com/user-attachments/assets/9cbc9180-49ab-4089-a5ec-8f98b91a5d40" alt="Тренировочная программа 1" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/731d79cf-adff-4de4-8cb0-a29b6cc1f54e" alt="Тренировочная программа 2" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/8e4f5ffc-130a-4dbb-8b12-02d88861bec1" alt="Тренировочная программа 3" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/51913024-fe0b-47cb-898c-448bc73710a4" alt="Тренировочная программа 4" width="70%">
</div>

### Возможности редактора

<div align="center">
  <img src="https://github.com/user-attachments/assets/28becc22-eb0f-4e97-8e88-8246f5dff57d" alt="Возможности редактора 1" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/1b6d8767-09b5-492c-9dab-d5fa8d5c6c42" alt="Возможности редактора 2" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/5c25e441-81c4-4364-b117-e6f63da9d201" alt="Возможности редактора 3" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/86f5e036-56e0-4850-b57f-bdbc8e72ef8b" alt="Возможности редактора 4" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/beca35a3-0fcc-4aa9-9bc9-441020e57d23" alt="Возможности редактора 5" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/e9c0900a-1a87-4b8e-a17c-b9a5c96ef881" alt="Возможности редактора 6" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/6cf9ade0-6e98-4385-a59c-e78de0bee859" alt="Возможности редактора 7" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/ea950bd5-ebd3-4c38-a16e-5ece99625a1d" alt="Возможности редактора 8" width="70%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/b3fa142c-8134-4117-ae5d-a695211b0b97" alt="Возможности редактора 9" width="50%">
</div>
<div align="center">
  <img src="https://github.com/user-attachments/assets/167352f6-0c43-4ad1-add5-c8064321a1bd" alt="Возможности редактора 10" width="50%">
</div>
