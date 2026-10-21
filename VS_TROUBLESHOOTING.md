# Если Unity не открывает скрипты

Симптом: при двойном клике по `.cs` Visual Studio показывает «Неизвестная ошибка» и
`ActivityLog.xml`, а сцена `MainMenu` выглядит как чёрный экран с `Display 0`.

## Что уже исправлено в проекте

- `MenuBootstrap` вынесен в отдельный файл `Assets/PUPPET/Scripts/Core/MenuBootstrap.cs`.
- `MainMenu.unity` теперь ссылается на него по обычному GUID, а не через встроенный
  `MonoScript` (из-за него компонент выглядел как пустой `(Script)`).
- `PUPPET-Underground-Whisper.slnx` переименован в `.slnx.bak`: Visual Studio 17.0
  не умеет открывать новый формат `.slnx` из Unity 6.5.
- Проверка `PUPPET → Validate Project` подтверждает: `menuBootstrap=True`.

## Как открыть код

1. В Unity: **Edit → Preferences → External Tools**.
2. **External Script Editor** → выберите **Visual Studio 2022**.
   Если VS не открывается — выберите **Visual Studio Code** или **Rider**.
3. Нажмите **Regenerate project files**.
4. Если VS 2022 версии 17.0 — обновите её до **17.14+**.
   Unity 6000.5 генерирует `.slnx`, который старые VS не понимают.
5. Вручную откройте `PUPPET Underground Whisper.sln` (не `.slnx`).

## Быстрая альтернатива

Откройте любую `.cs` из `Assets/PUPPET/Scripts` в **VS Code**, **Rider** или даже
блокноте — это обычные C#-файлы. Для комфортной работы с Unity лучше VS Code
с расширением C# или Rider.

## Запуск меню

- Откройте `Assets/PUPPET/Scenes/MainMenu.unity`.
- Нажмите **Play**.
- Или меню Unity: **PUPPET → Open Main Menu Scene**.
- `Display 0` без камеры — это признак того, что `MenuBootstrap` не запустился;
  после исправления сцены камера и меню создаются в `GameRoot.EnsureExists()`.
