# TechStore.Tests

Proyecto independiente de pruebas para el Examen Práctico 3.

## Cobertura

- Pruebas de ProductoService:
  - Consultar productos.
  - Obtener producto por ID.
  - Registrar producto.
  - Editar producto.
  - Eliminar producto.
  - Escenarios con registros inexistentes.
- Pruebas de ProductosController con Moq:
  - Setup.
  - Returns / ReturnsAsync.
  - Verify.
  - Validación de acciones y redirecciones.
- Prueba de integración:
  - ProductoService + Entity Framework Core + SQLite en memoria.
  - Flujo completo de registro, consulta, edición y eliminación.

## Herramientas

- xUnit.
- Moq.
- Entity Framework Core InMemory.
- SQLite en memoria para integración.

## Ejecución

En Visual Studio:

1. Compilar la solución.
2. Abrir Prueba > Explorador de pruebas.
3. Seleccionar Ejecutar todas.
4. Verificar que todas las pruebas aparezcan correctas.
