# Deployment notes

**PUPPET: Шёпот Подземного Города** — вертикальный срез 0.9, Unity 6000.5.8f1 (URP 2D).

## Запуск

- `Start_PUPPET.bat` или `Build\PUPPET_UndergroundWhisper.exe`.
- Сохранение: `%USERPROFILE%\AppData\LocalLow\Darknessings\PUPPET_ Шёпот Подземного Города\puppet_save.json`.

## Unity

- Открыть проект в Unity Hub (6000.5.8f1).
- Сцена входа: `Assets/PUPPET/Scenes/MainMenu.unity` (индекс 0). Меню `PUPPET → Open Main Menu Scene`.
- Игровые сцены: `Workshop`, `UpperCity`, `Library`, `Hospital`, `Factory`, `Core`.
- `PUPPET → Generate Content` пересобирает ArtLibrary, ScriptableObject-данные и сцены.
- `PUPPET → Build Windows` собирает `Build/PUPPET_UndergroundWhisper.exe`.

## Что добавлено в проект

- `Assets/PUPPET/` — весь код, данные, сцены и 128 процедурных спрайтов.
- `Build/` — готовый Windows-билд и скриншоты автотура.
- `Documentation/` — техдок, руководство, лор, баланс, презентация, PDF, изображения и сценарий демо.
- `README.md` — описание и управление.
- `ProjectSettings/EditorBuildSettings.asset` — список сцен PUPPET. Старый файл сохранён как `EditorBuildSettings.asset.before_puppet.bak`.
- Сцены из `Assets/!MyProject` (старый прототип) не удалялись; они просто не входят в список сборки PUPPET.

## Проверка

Сборка завершилась без ошибок компиляции. Автотур проверил движение, бой, медитацию, тьму,
смерть и возрождение, кнопку/дверь, верёвку, баррикаду, босса и финал; сохранение и продолжение
проверены отдельно.


