# 🎮 CMG Base Project

Một bộ khung Unity cơ bản

---
## Chú ý khi build game:
- Package name: điền thông tin cung cấp sau
- Scripting Backend: IL2CPP
- Target architectures: armv7 và arm64
- Target api level: api 35
- Minimum api: api level 24
- Version name: ví dụ - 1.0.0
- Version code: đặt theo quy tắc Năm-tháng-ngày-số bản build trong ngày (ví dụ: 2025072401), mỗi lần build tăng version lên
- Tạo keystore đặt vào trong thư mục BuildInfo, lưu keystore lại đẩy lên git.
- Tạo file info.txt trong thư mục BuildInfo. Lưu thông tin keystore lại ( lưu alias name, password)
- Nhập thông tin sdk voodoo: sẽ được cung cấp theo link này sau (https://drive.google.com/drive/folders/1xsucNln9NRkggFETJ2KMRteLlYfAV1i2)

## 📁 Quy ước đặt tên Asset

| Loại Asset           | Tiền tố     | Ví dụ                         |
|----------------------|-------------|-------------------------------|
| Sprite               | `spr_`      | `spr_coin`                    |
| UI Sprite            | `ui_`       | `ui_popupmission_title`      |
| Texture              | `tex_`      | `tex_block`                   |
| 3D Model             | `model_`    | `model_block`                 |
| Material             | `mat_`      | `mat_block`                   |
| Sound Effect (SFX)   | `sfx_`      | `sfx_win`                     |
| Background Music     | `bgm_`      | `bgm_gameplay`                |
| Animation Clip       | `anim_`     | `anim_jump`                   |
| Animator Controller  | `ac_`       | `ac_player`                   |
| Prefab               | `prefab_`   | `prefab_block`                |
| ScriptableObject     | `so_`       | `so_config_game`              |
| Shader               | `shader_`   | `shader_liquid`               |
| Font                 | `font_`     | `font_time_new_romans`       |
| Timeline Asset       | `tl_`       | `tl_action`                   |

---

## 🧩 Gọi UI View

```csharp
UISetting UISetting => GameViewsManager.Instance.GetView<UISetting>();
```

---

## 📌 Lấy State hiện tại của Game

```csharp
var inGameState = GameStateManager.Instance.GetCurrentState<InGameState>();
inGameState.IsEndLevel = true;
```

---

## 🔧 Các hàm chức năng có sẵn

| Chức năng     | Cách sử dụng                                                    |
|---------------|-----------------------------------------------------------------|
| Haptic        | `HapticController.Instance.PlayHaptic();`                       |
| Outline       | `OutlineController.Instance.AddOutline(gameObject);`           |
|               | `OutlineController.Instance.RemoveOutline(gameObject);`        |
| Sound (SFX)   | `AudioController.Instance.PlaySound(1);`                        |
| Effect (VFX)  | `EffectController.Instance.SpawnFx(1, Vector3.zero, 2f);`       |

---

## 🧱 Design Pattern đã áp dụng

- Singleton  
- Observer  
- Object Pooling  

---

## 🧰 Tiện ích

Một số tính năng mở rộng có thể được tìm thấy trong thư mục `Ulti/`.
Bổ sung thêm package UnmaskForUGUI, hỗ trợ làm tutorial dạng nhìn xuyên,target vào đối tượng (https://github.com/mob-sakai/UnmaskForUGUI)

---


# AUDIO MANAGER

B1: MAPPING KEY AND SFX
 - Tạo key trong so_audio_clip_data (in folder SO), điền ở "Audio ID Library" xong bấm "generate enums" để lưu key
 - Chọn key và sfx tương ứng ở trong "SFX" và "Background"

B2: USING
 - SFX: có các mode 
  + PlaySfx: phát 1 lần
  + PlaySfxRepeat: mỗi lần gọi sẽ phát lần lượt tất cả các clip trong list và lặp lại clip đầu tiên khi phát hết list
  + ResetRepeatSfx: reset về clip đầu tiên của key
  + ResetAllRepeatSfx: reset toàn bộ key về clip đầu
  + PlaySfxLoop: lặp lại 1 sfx
  + StopLoopSfx: dừng lặp sfx
  + PlayBackground: phát nhạc nền
  + StopBackground: dừng phát nhạc nền
  + SetSfxVolume: từ 0 tới 1 (1 là âm lượng lớn nhất)
  + SetBackgroundVolume: từ 0 tới 1 (1 là âm lượng lớn nhất)

# EFFECT MANAGER
 + tìm đến file SO_Vfx
 + điền tên vfx vào mảng Vfx Names
 + nhấn Generate Enum để tạo enum mới với tên tương ứng
 + kéo prefab tương ứng với enum vào cột Vfx Prefab
 + Dùng class EffectController để gọi effect theo enum tương ứng



