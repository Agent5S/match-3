# Match 3

Es un ejemplo básico de cómo implementar un juego de match 3

## Modalidad de juego

Se puede intercambiar la posición cualquier par de celdas adyacentes. Al juntar 3 o más celdas iguales desaparecen y se obtiene la cantidad de puntos equivalente a las celdas que desaparecen. Se debe obtener más de 60 puntos antes que acabe el tiempo.

## Algunas cosas por hacer

Agregar arte, animar los intercambios de las celdas, agregar efectos de sonido y música, agregar botón de pausa. Utilizar un object pool para reciclar los personajes que han sido eliminados.

## Errores

La generación del nivel debería prevenir que hayan 3 celdas iguales adyacentes **al iniciar el nivel**. Sin embargo esta funcionalidad tiene una falla que permite que en ciertas ocasiones hayan 3 celdas iguales adyacentes **verticalmente**. Intentaré arreglar este error si tengo más tiempo.
