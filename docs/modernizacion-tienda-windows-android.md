# Plan de modernización: UWP ➜ stack moderno con Microsoft Store + Android

Este documento propone una migración **incremental** para `MisNotas10` desde UWP hacia una arquitectura actual, manteniendo el soporte de Microsoft Store (incluyendo estado **comprada / trial**) y añadiendo Android.

## Recomendación principal

Usar **Uno Platform (WinUI + .NET 10)** como estrategia principal para este caso, con fallback temporal a .NET 8 solo si alguna dependencia crítica aún no está lista.

### Por qué Uno y no solo MAUI

- La app actual es XAML/UWP; Uno mantiene un modelo XAML/WinUI muy cercano y reduce fricción de migración.
- Permite target Windows y Android con un alto nivel de reutilización de vistas y ViewModels.
- En Windows, puede empaquetarse con MSIX y publicarse en Microsoft Store.

> Alternativa válida: .NET MAUI si se quiere priorizar ecosistema MAUI. Pero para una app nacida en UWP con bastante XAML, Uno suele requerir menos reescritura visual.

---

### Sobre .NET 10 vs .NET 8

Dado tu objetivo, **sí tiene sentido priorizar .NET 10** por vigencia de plataforma y horizonte de soporte.

Usaría esta regla:

- Objetivo principal: `net10.0` (y `net10.0-windows...` en el target Windows).
- Excepción temporal: mantener `net8.0` solo si un paquete crítico (por ejemplo, SDKs de terceros o librerías internas) aún no soporta net10 de forma estable.
- Salida del modo temporal: planificar la retirada de net8 en una iteración corta con fecha comprometida.

Así evitas deuda técnica nueva y mantienes una ruta de actualización limpia.


## ¿Qué aporta Uno frente a MAUI Hybrid y otras opciones?

Para esta app concreta (UWP/XAML + requisito fuerte de Store/licencias), Uno destaca por ventajas prácticas que otras rutas no siempre ofrecen con el mismo coste de migración.

### Comparativa rápida

| Opción | Qué reutilizas mejor desde UWP | UI nativa por plataforma | Coste de migración desde XAML UWP | Riesgo para paridad visual/funcional |
|---|---|---|---|---|
| **Uno Platform (WinUI)** | XAML, patrones MVVM, recursos y parte de controles | Sí | **Bajo/Medio** | **Bajo/Medio** |
| **.NET MAUI (XAML MAUI)** | MVVM/lógica, pero no XAML UWP 1:1 | Sí | Medio/Alto | Medio |
| **MAUI Hybrid (BlazorWebView)** | Lógica .NET y parte backend | Parcial (UI web embebida) | Alto (rehacer UI a Razor/CSS) | Medio/Alto |
| **Avalonia** | Lógica MVVM | Sí | Medio/Alto (XAML distinto) | Medio |
| **WinUI 3 solo Windows** | Mucha parte UWP/WinUI | Sí (solo Windows) | Bajo/Medio | Bajo (pero sin Android) |

### Diferenciales concretos de Uno en tu caso

1. **Continuidad real de XAML/WinUI**
   - Si ya tienes una base UWP con muchas vistas XAML, Uno reduce la reescritura de UI respecto a MAUI/Hybrid.

2. **Un solo stack visual para Windows y Android**
   - Evitas tener Windows en XAML y Android en otra tecnología (p. ej. Razor/HTML en Hybrid), lo que simplifica mantenimiento.

3. **Encaje natural con licencias Microsoft Store en Windows**
   - El problema de trial/pago se resuelve en capa de plataforma Windows (StoreContext) sin forzar cambios de arquitectura en toda la UI.

4. **Migración incremental más segura**
   - Puedes portar pantalla por pantalla y comparar comportamiento con la versión UWP existente.

### ¿Cuándo MAUI Hybrid sí podría ser mejor?

- Si vuestro equipo domina fuertemente **Blazor/Razor** y quiere compartir UI web con otros canales.
- Si aceptáis reescribir UI y priorizáis productividad web por encima de continuidad XAML.

### Regla práctica de decisión

- **Si priorizas minimizar riesgo y reaprovechar UWP/XAML:** Uno suele ganar.
- **Si priorizas stack web (Razor/CSS) y ya tenéis ese músculo:** MAUI Hybrid puede encajar mejor.
- **Si Android no fuese requisito:** WinUI 3 puro sería la transición más directa en Windows.

## Requisito crítico: conservar compra y trial en Microsoft Store

### Lo importante de negocio

Para no “romper” la base instalada, lo esencial no es UWP en sí, sino:

1. **Mantener la misma identidad/listing de producto en Store** cuando publiques la versión moderna.
2. En Windows, seguir consultando licencia con `Windows.Services.Store.StoreContext`.
3. Encapsular la lógica en un servicio de dominio para que Android use su propio proveedor (Google Play Billing), sin mezclar reglas por plataforma.

### Servicio propuesto de licenciamiento

Define una interfaz compartida:

```csharp
public enum EntitlementState
{
    Unknown,
    Trial,
    Purchased
}

public interface ILicenseService
{
    Task<EntitlementState> GetEntitlementAsync(CancellationToken ct = default);
}
```

#### Implementación Windows (StoreContext)

- Usar `StoreContext.GetDefault()`.
- Consultar `GetAppLicenseAsync()`.
- Mapear a dominio:
  - `IsActive == true && IsTrial == true` ➜ `Trial`
  - `IsActive == true && IsTrial == false` ➜ `Purchased`
  - resto ➜ `Unknown` (o Trial restringido según tu política)

#### Implementación Android

- Usar Google Play Billing (suscripción o compra one-time según modelo comercial).
- Mapear estados al mismo enum (`Trial`/`Purchased`) para que la UI/negocio sea agnóstica de plataforma.

---

## Arquitectura objetivo

Separar por capas para facilitar migración:

- `MisNotas.Core`
  - Modelos (`Nota`, etc.)
  - Casos de uso
  - Interfaces (`INotaRepository`, `ILicenseService`, `IBackupService`)
- `MisNotas.Infrastructure`
  - SQLite
  - OneDrive (Graph)
  - Implementaciones de servicios
- `MisNotas.UI` (Uno/WinUI)
  - Vistas XAML
  - ViewModels (MVVM)

Así podrás mover código actual (Modelos, ViewModels, Servicios) por fases.

---

## Plan de migración por fases (bajo riesgo)

## Fase 0 — Preparación

1. Congelar funcionalidades nuevas en UWP (solo fixes urgentes).
2. Documentar reglas de negocio actuales (trial/pago, backup OneDrive, etc.).
3. Identificar dependencias UWP-only.

## Fase 1 — Extraer núcleo compartido

1. Crear proyecto `Core` .NET 10.
2. Mover:
   - `Models/Nota.cs`
   - lógica de ViewModels reusable
   - contratos de servicios
3. Mantener UWP funcionando mediante adaptadores.

## Fase 2 — Crear app nueva Windows + Android (Uno)

1. Crear solución Uno con targets Windows y Android.
2. Referenciar `Core`.
3. Portar pantallas clave primero:
   - Lista de notas
   - Edición de nota
   - Ajustes/licenciamiento

## Fase 3 — Licencias y monetización

1. Implementar `ILicenseService` Windows con `StoreContext`.
2. Verificar que build Windows publicada en Store usa el mismo producto.
3. Implementar `ILicenseService` Android con Billing.
4. Probar matrices: usuario comprado, trial, no autenticado, offline.

## Fase 4 — Datos y backup

1. Reutilizar SQLite.
2. Revisar OneDrive (preferible Microsoft Graph moderno).
3. Migración de esquema si hace falta.

## Fase 5 — Publicación gradual

1. Publicar primero versión Windows moderna (misma ficha de app si aplica).
2. Monitorear crashes/telemetría.
3. Publicar Android.

---

## Requisito adicional clave: mantener soporte multi-idioma

La versión actual ya está publicada en múltiples idiomas, por lo que la migración debe preservar este comportamiento desde el primer release moderno.

### Estrategia recomendada de localización

1. **Mantener estructura de recursos por cultura**
   - Reutilizar el catálogo actual de cadenas (`Strings/<culture>/Resources.resw`) y consolidarlo en el nuevo proyecto.

2. **No mezclar i18n de plataforma con textos hardcodeados**
   - Toda cadena visible en UI debe salir de recursos localizados.

3. **Paridad funcional de idiomas en Windows y Android**
   - Definir una lista mínima de idiomas de salida (idealmente los mismos de la app actual).

4. **Pruebas de regresión de localización**
   - Verificar idioma por defecto, fallback, formato de fecha/número y textos largos en layouts críticos.

### Checklist i18n para publicación

- [ ] El idioma del sistema selecciona recursos correctos.
- [ ] Fallback a idioma por defecto cuando falta una cadena.
- [ ] No hay cadenas quemadas en vistas nuevas.
- [ ] Metadatos de tienda (descripción/capturas) coherentes con idiomas soportados.

---

## Riesgos y mitigaciones

- **Riesgo:** discrepancias entre estado de compra legacy y versión nueva.
  - **Mitigación:** pruebas en sandbox Store + producción con cuentas reales de prueba.

- **Riesgo:** diferencias de comportamiento XAML al portar.
  - **Mitigación:** migración por pantallas y tests de UI críticos.

- **Riesgo:** APIs UWP no disponibles tal cual.
  - **Mitigación:** capa de abstracción por plataforma desde el inicio.

---

## Checklist de compatibilidad para tu caso

- [ ] Mantener identidad de producto en Microsoft Store al publicar la app Windows moderna.
- [ ] Conservar lógica funcional de trial/pago en Windows con `StoreContext`.
- [ ] Añadir `ILicenseService` multiplataforma.
- [ ] Definir monetización Android equivalente (compra única/suscripción/trial).
- [ ] Alinear UX de “premium desbloqueado” entre plataformas.

---

## Decisión recomendada (resumen ejecutivo)

Para este proyecto concreto (UWP XAML + requisito fuerte de Store + deseo de Android), la ruta más segura suele ser:

1. **Uno Platform sobre .NET 10** para Windows + Android (o transición temporal en .NET 8 si hay bloqueo de dependencias).
2. **Abstracción de licencias** desde el dominio.
3. **StoreContext en Windows** para preservar compra/trial de Microsoft Store.
4. **Google Billing en Android** para monetización equivalente.

Esto te permite modernizar sin perder lo ya vendido en Store, y abrir Android sin duplicar toda la app.
