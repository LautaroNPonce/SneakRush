# SneakRush — Sistema de E-commerce

Plataforma web de e-commerce desarrollada en ASP.NET MVC, enfocada en seguridad, control de accesos dinámico y arquitectura escalable.

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/shop.svg" width="20"/> Descripción

SneakRush es una aplicación web diseñada para la gestión integral de una tienda de zapatillas.  
Integra mecanismos avanzados de seguridad, un sistema de permisos dinámico basado en el patrón Composite y herramientas administrativas robustas.

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/shield-lock.svg" width="20"/> Seguridad y acceso

- Autenticación de usuarios (Administrador, Webmaster y Cliente)  
- Encriptación de contraseñas con SHA256  
- Políticas de contraseñas y restablecimiento obligatorio  
- Control de acceso basado en permisos dinámicos  

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/diagram-3.svg" width="20"/> Sistema de permisos (Patrón Composite)

- Implementación del patrón Composite  
- Organización de permisos en:
  - Familias (roles)  
  - Patentes (acciones)  
- Asignación dinámica de permisos a usuarios  
- Validación en tiempo real en controladores y vistas  

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/cart.svg" width="20"/> Tienda online

- Visualización de productos  
- Filtros por categoría y marca  
- Carrito de compras  
- Historial de compras  

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/receipt.svg" width="20"/> Gestión de ventas

- Registro de ventas con detalle  
- Exportación e importación en formato XML  
- Persistencia en base de datos  

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/tools.svg" width="20"/> Panel de administración

- Gestión de:
  - Productos  
  - Categorías  
  - Marcas  
- Subida de imágenes de productos  
- Control completo del sistema  

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/journal-text.svg" width="20"/> Bitácora

- Registro de acciones del sistema:
  - Inicio de sesión  
  - Operaciones CRUD  
  - Errores  
- Seguimiento de actividad por usuario  

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/database-check.svg" width="20"/> Integridad de datos

- Implementación de DVH (Dígito Verificador Horizontal)  
- Implementación de DVV (Dígito Verificador Vertical)  
- Verificación automática al iniciar sesión  
- Detección de corrupción de datos  
- Opciones de restauración y recalculado  

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/hdd-stack.svg" width="20"/> Backup y Restore

- Generación de backups de la base de datos  
- Restauración desde archivo  
- Controlado mediante permisos específicos  

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/cloud.svg" width="20"/> Web Service

- Servicio XML para consulta de productos con stock  

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/code-slash.svg" width="20"/> Tecnologías utilizadas

- **Backend:** C# — ASP.NET MVC (.NET Framework)  
- **Frontend:** HTML, CSS, Bootstrap, JavaScript, jQuery  
- **Base de datos:** SQL Server  
- **Arquitectura:** Capas (BE, BLL, DAL, UI)  
- **Seguridad:** SHA256 + AES  
- **Patrón de diseño:** Composite  

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/boxes.svg" width="20"/> Arquitectura del sistema

El proyecto está estructurado en capas:

- **BE (Business Entities):** Entidades del dominio  
- **BLL (Business Logic Layer):** Lógica de negocio  
- **DAL (Data Access Layer):** Acceso a datos  
- **UI (MVC):** Controladores y vistas  

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/key.svg" width="20"/> Modelo de permisos

SneakRush implementa un sistema de permisos jerárquico y flexible:

- Cada usuario posee una estructura de permisos propia  
- Validación de permisos en tiempo real  
- Permite restringir acciones como:
  - Crear productos  
  - Eliminar usuarios  
  - Realizar backups  
  - Acceder a la bitácora  

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/star.svg" width="20"/> Características destacadas

- Control total de seguridad y accesos  
- Arquitectura escalable por capas  
- Integridad de datos con DVH y DVV  
- Sistema de permisos profesional y dinámico  
- Registro completo de actividad (bitácora)  
- Funcionalidades avanzadas de administración  

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/globe.svg" width="20"/> Portafolio

👉 [https://sneakrush-web.vercel.app/](https://sneakrush-web.vercel.app/)

---

## <img src="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/icons/person.svg" width="20"/> Autor

Desarrollado por Lautaro N Ponce

---
