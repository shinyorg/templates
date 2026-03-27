---
name: shiny-contactstore
description: Generate code using Shiny.Maui.ContactStore for cross-platform device contact access with CRUD, LINQ queries, and MAUI permissions
auto_invoke: true
triggers:
  - contact store
  - contacts
  - IContactStore
  - ContactStore
  - AddContactStore
  - ContactPermission
  - ContactReadPermission
  - ContactWritePermission
  - Shiny.Maui.ContactStore
  - device contacts
  - read contacts
  - write contacts
  - contact query
  - contact LINQ
---

# Shiny.Maui.ContactStore Skill

You are an expert in Shiny.Maui.ContactStore, a cross-platform .NET MAUI library for accessing device contacts on Android and iOS.

## When to Use This Skill

Invoke this skill when the user wants to:
- Access device contacts (read, create, update, delete)
- Query contacts with LINQ
- Set up contact permissions using MAUI permissions
- Register the contact store in DI
- Work with contact models (phones, emails, addresses, etc.)

## Library Overview

**GitHub**: https://github.com/shinyorg/contactstore
**NuGet**: `Shiny.Maui.ContactStore`
**Namespace**: `Shiny.Maui.ContactStore`

Shiny.Maui.ContactStore provides:
- Full CRUD operations on device contacts
- LINQ query support with native translation (Android content provider queries, iOS CNContact predicates)
- MAUI permission classes for requesting contact access
- Dependency injection integration
- AOT and trimmer compatible

## Setup

### 1. Install NuGet Package
```bash
dotnet add package Shiny.Maui.ContactStore
```

### 2. Register in MauiProgram.cs
```csharp
using Shiny.Maui.ContactStore;

builder.Services.AddContactStore();
```

### 3. Platform Permissions

**Android** — Add to `AndroidManifest.xml`:
```xml
<uses-permission android:name="android.permission.READ_CONTACTS" />
<uses-permission android:name="android.permission.WRITE_CONTACTS" />
```

**iOS** — Add to `Info.plist`:
```xml
<key>NSContactsUsageDescription</key>
<string>This app needs access to your contacts.</string>
```

## Permissions

The library provides a MAUI permission class `ContactPermission` that wraps both read and write contact permissions.

Use the extension methods on `IContactStore`:

```csharp
// Request permissions (triggers OS prompt if needed)
var status = await contactStore.RequestPermssionsAsync();
if (status != PermissionStatus.Granted)
{
    // Handle denied
    return;
}

// Check current status without prompting
var status = await contactStore.CheckPermissionStatusAsync();
```

### Android Permission Results
- `PermissionStatus.Granted` — both read and write access granted
- `PermissionStatus.Limited` — only read or only write granted (not both)
- `PermissionStatus.Denied` — neither read nor write granted

### iOS Permission Results
- `PermissionStatus.Granted` — contacts access authorized
- `PermissionStatus.Denied` — contacts access denied
- `PermissionStatus.Restricted` — contacts access restricted

## API Reference

### IContactStore Interface

```csharp
public interface IContactStore
{
    Task<IReadOnlyList<Contact>> GetAll(CancellationToken ct = default);
    Task<Contact?> GetById(string contactId, CancellationToken ct = default);
    IQueryable<Contact> Query();
    Task<string> Create(Contact contact, CancellationToken ct = default);
    Task Update(Contact contact, CancellationToken ct = default);
    Task Delete(string contactId, CancellationToken ct = default);
}
```

### Extension Methods

```csharp
// Permission extensions
Task<PermissionStatus> contactStore.RequestPermssionsAsync();
Task<PermissionStatus> contactStore.CheckPermissionStatusAsync();

// Query extensions
Task<IReadOnlyList<char>> contactStore.GetFamilyNameFirstLetters(CancellationToken ct = default);
```

### Query with LINQ

The library translates LINQ predicates to native queries where possible, with in-memory fallback.

```csharp
// Filter by name
var results = contactStore.Query()
    .Where(c => c.GivenName.Contains("John"))
    .ToList();

// Filter by phone number
var results = contactStore.Query()
    .Where(c => c.Phones.Any(p => p.Number.Contains("555")))
    .ToList();

// Filter by email
var results = contactStore.Query()
    .Where(c => c.Emails.Any(e => e.Address.Contains("@example.com")))
    .ToList();

// Combine filters
var results = contactStore.Query()
    .Where(c => c.GivenName.StartsWith("J") && c.FamilyName.Contains("Smith"))
    .ToList();

// Paging
var page = contactStore.Query()
    .Where(c => c.FamilyName.StartsWith("A"))
    .Skip(10)
    .Take(20)
    .ToList();
```

**Supported operations:** `Contains`, `StartsWith`, `EndsWith`, `Equals`

**Filterable properties:** `GivenName`, `FamilyName`, `MiddleName`, `NamePrefix`, `NameSuffix`, `Nickname`, `DisplayName`, `Note`

**Filterable collections:** `Phones` (by `Number`), `Emails` (by `Address`)

### Create a Contact

```csharp
var contact = new Contact
{
    GivenName = "John",
    FamilyName = "Doe",
    Note = "Met at conference"
};
contact.Phones.Add(new ContactPhone("555-1234", PhoneType.Mobile));
contact.Emails.Add(new ContactEmail("john@example.com", EmailType.Work));

string id = await contactStore.Create(contact);
```

### Update a Contact

```csharp
var contact = await contactStore.GetById(id);
contact.GivenName = "Jane";
await contactStore.Update(contact);
```

### Delete a Contact

```csharp
await contactStore.Delete(contactId);
```

## Models

### Contact

| Property       | Type                        |
|----------------|-----------------------------|
| Id             | `string?`                   |
| NamePrefix     | `string?`                   |
| GivenName      | `string?`                   |
| MiddleName     | `string?`                   |
| FamilyName     | `string?`                   |
| NameSuffix     | `string?`                   |
| Nickname       | `string?`                   |
| DisplayName    | `string`                    |
| Note           | `string?`                   |
| Organization   | `ContactOrganization?`      |
| Photo          | `byte[]?`                   |
| Thumbnail      | `byte[]?`                   |
| Phones         | `List<ContactPhone>`        |
| Emails         | `List<ContactEmail>`        |
| Addresses      | `List<ContactAddress>`      |
| Dates          | `List<ContactDate>`         |
| Relationships  | `List<ContactRelationship>` |
| Websites       | `List<ContactWebsite>`      |

### Enums

**PhoneType:** Home, Mobile, Work, FaxWork, FaxHome, Pager, Other, Custom

**EmailType:** Home, Work, Other, Custom

**AddressType:** Home, Work, Other, Custom

**ContactDateType:** Birthday, Anniversary, Other, Custom

**RelationshipType:** Father, Mother, Parent, Brother, Sister, Child, Friend, Spouse, Partner, Assistant, Manager, Other, Custom

## iOS Notes & Relations Entitlement

Reading `Note` and `Relationships` on iOS requires the `com.apple.developer.contacts.notes` entitlement. The library auto-detects this at runtime. If absent, `Note` returns `null` and `Relationships` is empty.

## Best Practices

1. **Always request permissions first** — use `contactStore.RequestPermssionsAsync()` before any CRUD operation
2. **Use LINQ queries for filtering** — prefer `Query().Where(...)` over `GetAll()` when filtering, as it uses native queries
3. **Check for Limited on Android** — `PermissionStatus.Limited` means partial access (read-only or write-only)
4. **Handle iOS entitlements gracefully** — Notes and Relations silently return empty without the entitlement
5. **Use primary constructors** — inject `IContactStore` via primary constructor
