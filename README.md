# Feed-Forward-Neural-Net
В данном проекте реализована простая нейронная сеть, которая осуществляет распознование флагов стран. По своей архитектуре нейросеть является многослойным персептроном. Она состоит из входного слоя, 3 скрытых слоёв и выходного слоя. В качестве функции активации использовалась сигмоидальная функция. На каждом из скрытых слоёв находится по 3 нейрона, хотя нейросеть в этом месте легко масштабируема и разработчик может самостоятельно задать число нейронов на каждом скрытом слое. На выходном слое в текущей реализации содержится 2 нейрона. Обучение нейронной сети происходит с помощью метода обратного распространения ошибки. Для этого для каждого нейрона, начиная с выходного слоя, происходит вычисление "Дельты", с помощью которой затем происходит корректировка весов соответствующих нейронов сети.

___
### Руководство пользователя: 
Перед запуском программы необходимо входную выборку данных разделить на три непересекающихся подмножества: для обучения, тестирования и верификации. Каждый набор данных  должен быть помещен в соответствующую папку. После запуска программы пользователь должен выбрать каталог, в котором находятся изображения, после чего программа будет готова к использованию. Неоходимо учесть, что нейронная сеть изначально не обучена. При нажатии на кнопку "Обучить" начнётся обучение нейронной сети, при нажатии "Тестировать" - тестирование, при нажатии "Верифицировать" - процесс верификации. По выполнению каждого из этапов построится график изменения ошибки обучения/тестирования/верификации.
