# 🛍️ SneakRush

SneakRush es un sistema web de e-commerce desarrollado en ASP.NET MVC, orientado a la gestión integral de una tienda de zapatillas. El proyecto integra seguridad avanzada, control de accesos mediante permisos dinámicos (patrón Composite) y herramientas de administración robustas.

---

## 🚀 Funcionalidades principales

### 🔐 Seguridad y acceso

* Autenticación de usuarios (Administrador, Webmaster y Cliente)
* Encriptación de contraseñas (SHA256)
* Política de contraseñas y restablecimiento obligatorio
* Control de acceso basado en permisos dinámicos

### 🧩 Sistema de permisos (Composite)

* Implementación del patrón Composite
* Permisos organizados en:

  * Familias (roles)
  * Patentes (acciones)
* Asignación dinámica de permisos a usuarios
* Validación de permisos en controladores y vistas

### 🛒 Tienda online

* Visualización de productos
* Filtros por categoría y marca
* Carrito de compras
* Registro de ventas e historial

### 🧾 Gestión de ventas

* Registro de ventas con detalle
* Exportación e importación de ventas en XML
* Persistencia en base de datos

### 🧰 Panel de administración

* Gestión de:

  * Productos
  * Categorías
  * Marcas
* Subida de imágenes de productos
* Control total del sistema

### 📊 Bitácora

* Registro de acciones del sistema:

  * Inicio de sesión
  * Operaciones CRUD
  * Errores
* Seguimiento de actividad por usuario

### 🧮 Integridad de datos

* Implementación de DVH y DVV
* Verificación automática al iniciar sesión
* Detección de corrupción de datos
* Opciones de restauración y recalculado

### 💾 Backup y Restore

* Generación de backups de la base de datos
* Restauración desde archivo
* Controlado por permisos específicos

### 🌐 WebService

* Servicio XML para consulta de productos con stock

---

## 🛠️ Tecnologías utilizadas

* **Backend:** C# – ASP.NET MVC (.NET Framework)
* **Frontend:** HTML, CSS, Bootstrap, JavaScript, jQuery
* **Base de datos:** SQL Server
* **Arquitectura:** Capas (BE, BLL, DAL, UI)
* **Seguridad:** SHA256 + AES
* **Patrones:** Composite (permisos)

---

## 🧱 Arquitectura del sistema

El proyecto está estructurado en capas:

* **BE (Business Entities):** Entidades del dominio
* **BLL (Business Logic Layer):** Lógica de negocio
* **DAL (Data Access Layer):** Acceso a datos
* **UI (MVC):** Controladores y vistas

---

## 🔐 Sistema de permisos

SneakRush implementa un sistema de permisos flexible basado en el patrón Composite:

* Cada usuario posee una estructura de permisos jerárquica
* Se validan permisos en tiempo real
* Permite restringir acciones específicas como:

  * Crear productos
  * Eliminar usuarios
  * Realizar backups
  * Ver bitácora

---

## 📌 Características destacadas

✔ Control total de seguridad y accesos
✔ Arquitectura escalable por capas
✔ Implementación de integridad de datos (DVH/DVV)
✔ Sistema de permisos profesional y dinámico
✔ Registro completo de actividad (bitácora)
✔ Funcionalidades avanzadas de administración

---

## 📷 Capturas (opcional)

*Agregar imágenes del sistema aquí*

---

## 📦 Instalación

1. Clonar el repositorio:

```
git clone https://github.com/TU-USUARIO/SneakRush.git
```

2. Configurar la cadena de conexión en `Web.config`

3. Ejecutar el script SQL de la base de datos

4. Abrir el proyecto en Visual Studio y ejecutar

---

## 👨‍💻 Autor

Desarrollado por **Lautaro Torres Pandolfo**
Estudiante de Negocios y Tecnología – ITBA

---

## 📄 Licencia

Este proyecto es de uso académico.
