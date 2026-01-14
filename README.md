# Příjmy a výdaje

Webová aplikace pro správu osobních financí zaměřená na **evidenci příjmů a výdajů**, řízení přístupových práv a automatizovaný import bankovních transakcí.

Aplikace je navržena s důrazem na **oddělení uživatelských dat**, bezpečnost a rozšiřitelnost.

Url: https://iaeapp.azurewebsites.net/

---

## Přehled aplikace

Každý uživatel pracuje výhradně se **svými vlastními transakcemi**.  
Po přihlášení má uživatel k dispozici **dashboard se čtyřmi grafy**, které poskytují přehled o jeho finanční situaci.

Aplikace podporuje více rolí s jasně definovanými oprávněními a umožňuje automatický import transakcí z bankovních výpisů.

---

## Obsah

- Přehled aplikace
- Autentizace a uživatelské účty
- Role a oprávnění
- Automatický import transakcí
- Technologie
- Bezpečnost
- Budoucí rozšíření

---

## Autentizace a uživatelské účty

Přihlášení probíhá pomocí **e-mailové adresy a hesla**.

Vytváření nových uživatelů je omezeno pouze na role:

- **Admin**
- **SuperAdmin**

### Testovací účty

**Admin**
- Email: `admin@admin.cz`
- Heslo: `Abcd1234`

**Uživatel**
- Email: `petr@petr.cz`
- Heslo: `Abcd1234`

> Přihlašovací údaje pro roli **SuperAdmin** nejsou z bezpečnostních důvodů veřejně dostupné.

---

## Role a oprávnění

Aplikace využívá **role-based access control (RBAC)**.

| Role        | Popis oprávnění |
|------------|------------------|
| Uživatel   | Správa vlastních transakcí, import transakcí |
| Admin      | Správa transakcí, kategorií, uživatelů a rolí |
| SuperAdmin | Plná administrátorská práva bez omezení |

### Detailní přehled oprávnění

| Funkce / Role | Uživatel | Admin | SuperAdmin |
|--------------|----------|-------|------------|
| CRUD transakcí | ✔ | ✔ | ✔ |
| Import transakcí | ✔ | ✔ | ✔ |
| Editace kategorií | | ✔ | ✔ |
| Mazání kategorií | | | ✔ |
| Vytváření uživatelů | | ✔ | ✔ |
| Editace uživatelů | | ✔ | ✔ |
| Mazání uživatelů | | | ✔ |
| Editace rolí | | ✔ | ✔ |
| Mazání rolí | | | ✔ |
| Správa SuperAdmin účtů | | | ✔ |

---

## Automatický import transakcí

Aplikace podporuje automatický import bankovních transakcí:

- **Banka:** ČSOB  
- **Formát:** XML (`.xml`)

### Průběh importu

1. Uživatel nahraje XML výpis z banky
2. Transakce jsou automaticky zpracovány
3. Nové položky jsou uloženy do databáze
4. Transakce jsou přiřazeny do kategorie **Import**
5. Kategorie lze následně upravit ručně

---

## Technologie

- **ASP.NET Core MVC**
- **Entity Framework Core**
- **SQL databáze**
- **Bootstrap**
- **XML parsing**

---

## Bezpečnost

- Oddělení dat jednotlivých uživatelů
- Role-based přístupová práva
- Omezení administrátorských operací
- Skryté přístupy k SuperAdmin účtu

---

## Budoucí rozšíření

Plánované nebo možné rozšíření aplikace:

- Podpora dalších bank a formátů výpisů
- Export dat (CSV / Excel)
- Detailnější statistiky a reporty
- Rozšíření dashboardu
- Vícejazyčná lokalizace

---

## Autor

Projekt byl vytvořen jako **osobní vývojový projekt** se zaměřením na:
- ASP.NET Core
- bezpečnost aplikací
- návrh role-based systémů
- práci s daty a XML

---

