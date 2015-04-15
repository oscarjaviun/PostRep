# PostRep
https://github.com/oscarjaviun/PostSolution
PostSolution
El sistema consta de dos soluciones, una para cliente y otra para servidor, llamadas TextTest y TextPostApi, respectivamente. Estos son los nombres de las soluciones. La solución TextTest contiene:

PostConsoleClient: Una aplicación de consola que contiene el cliente.
TextGenerationUtils: Es una librería de clases que contiene helpers para el manejo de los post. La solución TextPostApi contiene:
TextPostApi: Esta es una solución de tipo WEBApi que expone los endpoints para realizar el recibo de los textos.
TextUtils: Contiene una clase TextProcesor que se encarga de las reglas de negocio.
PersistenceManager: Se encarga de generar los resultados y almacenarlos para revisarlos posteriormente.
En general la arquitectura es cliente servidor a dos capas. Utiliza JSON para el transporte de datos. La WEBAPI debe ser publicada en IIS.
