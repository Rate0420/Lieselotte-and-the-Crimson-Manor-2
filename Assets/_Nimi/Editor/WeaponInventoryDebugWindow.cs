using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using EnchantmentName = EnchantmentDataBase.EnchantmentName;

namespace Nimi.Editor
{
    /// <summary>EditorのGUIで武器と付呪3枠を指定し、再生中の所持武器へ追加・保存する。</summary>
    public class WeaponInventoryDebugWindow : EditorWindow
    {
        private const string LayoutPath = "Assets/_Nimi/Editor/WeaponInventoryDebugWindow.uxml";
        [SerializeField] private WeaponTemplate _selectedWeapon;
        [SerializeField] private EnchantmentName[] _enchantmentNames =
            { EnchantmentName.なし, EnchantmentName.なし, EnchantmentName.なし };
        [SerializeField] private int[] _levels = { 1, 1, 1 };

        private readonly List<WeaponTemplate> _weapons = new();
        private readonly List<EnchantmentName> _enchantments = new();
        private readonly List<DropdownField> _enchantmentFields = new();
        private readonly List<IntegerField> _levelFields = new();
        private WeaponInventory _inventory;
        private Enchantment _database;
        private Enchantment _defaultDatabase;
        private DropdownField _weaponField;
        private Image _weaponIcon;
        private HelpBox _contextHelp;
        private HelpBox _resultHelp;
        private Button _addButton;

        [MenuItem("Tools/Nimi/所持武器を追加")]
        public static void Open()
        {
            var window = GetWindow<WeaponInventoryDebugWindow>("所持武器を追加");
            window.minSize = new Vector2(460, 390);
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            ObserveInventory(null);
        }

        /// <summary>UXMLの画面を読み込み、武器選択・付呪選択・レベル入力と追加ボタンを接続する。</summary>
        public void CreateGUI()
        {
            rootVisualElement.Clear();
            var layout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(LayoutPath);
            if (layout == null)
            {
                rootVisualElement.Add(new HelpBox("武器追加ウィンドウのレイアウトが見つかりません。", HelpBoxMessageType.Error));
                return;
            }
            layout.CloneTree(rootVisualElement);
            _weaponField = rootVisualElement.Q<DropdownField>("weaponField");
            _weaponIcon = rootVisualElement.Q<Image>("weaponIcon");
            _contextHelp = rootVisualElement.Q<HelpBox>("contextHelp");
            _resultHelp = rootVisualElement.Q<HelpBox>("resultHelp");
            _addButton = rootVisualElement.Q<Button>("addButton");
            _enchantmentFields.Clear();
            _levelFields.Clear();
            var rows = rootVisualElement.Q<VisualElement>("enchantmentRows");
            for (int i = 0; i < 3; i++)
            {
                int index = i;
                var row = new VisualElement();
                row.AddToClassList("enchantment-row");
                var enchantment = new DropdownField($"付呪 {i + 1}");
                enchantment.AddToClassList("enchantment-choice");
                var level = new IntegerField("Lv") { value = _levels[i], isDelayed = true };
                level.AddToClassList("enchantment-level");
                enchantment.RegisterValueChangedCallback(_ =>
                {
                    _enchantmentNames[index] = _enchantments[Mathf.Max(0, enchantment.index)];
                    UpdateLevel(index);
                    UpdateStatus();
                });
                level.RegisterValueChangedCallback(evt =>
                {
                    _levels[index] = Mathf.Max(1, evt.newValue);
                    level.SetValueWithoutNotify(_levels[index]);
                    UpdateStatus();
                });
                row.Add(enchantment);
                row.Add(level);
                rows.Add(row);
                _enchantmentFields.Add(enchantment);
                _levelFields.Add(level);
            }
            _weaponField.RegisterValueChangedCallback(_ =>
            {
                _selectedWeapon = _weaponField.index >= 0 && _weaponField.index < _weapons.Count
                    ? _weapons[_weaponField.index] : null;
                UpdateStatus();
            });
            _addButton.clicked += AddSelectedWeapon;
            var databases = AssetDatabase.FindAssets("t:Enchantment");
            if (databases.Length > 0)
                _defaultDatabase = AssetDatabase.LoadAssetAtPath<Enchantment>(AssetDatabase.GUIDToAssetPath(databases[0]));
            RefreshContext(true);
            // 再生開始・終了やシーン変更を検知するため、操作対象を定期的に確認する。
            rootVisualElement.schedule.Execute(() => RefreshContext()).Every(500);
        }

        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            RefreshContext(true);
        }

        /// <summary>操作対象のインベントリが変わったら、古い変更通知を解除して登録し直す。</summary>
        private void ObserveInventory(WeaponInventory inventory)
        {
            if (_inventory != null) _inventory.OnInventoryChanged -= UpdateStatus;
            _inventory = inventory;
            if (_inventory != null) _inventory.OnInventoryChanged += UpdateStatus;
        }

        /// <summary>再生状態やシーン変更に合わせて操作対象とデータベースを更新する。</summary>
        private void RefreshContext(bool force = false)
        {
            if (_weaponField == null) return;
            var inventory = Application.isPlaying ? WeaponInventory.Instance : null;
            var database = inventory != null ? inventory.defaultEnchantmentDB : _defaultDatabase;
            if (force || inventory != _inventory || database != _database)
            {
                ObserveInventory(inventory);
                _database = database;
                RefreshChoices();
            }
            UpdateStatus();
        }

        /// <summary>保存後に復元できる武器と、効果データのある付呪を選択肢へ設定する。</summary>
        private void RefreshChoices()
        {
            _weapons.Clear();
            if (_inventory != null)
            {
                // セーブの読込先に登録されている武器だけを候補にする。
                foreach (var weapon in new[] { _inventory.sword_ShieldTemplate, _inventory.greatswordTemplate, _inventory.rapierTemplate })
                    if (weapon != null && !_weapons.Contains(weapon)) _weapons.Add(weapon);
            }
            else
            {
                foreach (string guid in AssetDatabase.FindAssets("t:WeaponTemplate"))
                    _weapons.Add(AssetDatabase.LoadAssetAtPath<WeaponTemplate>(AssetDatabase.GUIDToAssetPath(guid)));
            }
            var weaponLabels = _weapons.ConvertAll(weapon => string.IsNullOrEmpty(weapon.displayName) ? weapon.name : weapon.displayName);
            _weaponField.choices = weaponLabels;
            if (!_weapons.Contains(_selectedWeapon)) _selectedWeapon = _weapons.Count > 0 ? _weapons[0] : null;
            _weaponField.SetValueWithoutNotify(_selectedWeapon != null ? weaponLabels[_weapons.IndexOf(_selectedWeapon)] : string.Empty);
            _weaponField.SetEnabled(_weapons.Count > 0);

            _enchantments.Clear();
            _enchantments.Add(EnchantmentName.なし);
            if (_database != null && _database.EnchantmentData != null)
                foreach (var data in _database.EnchantmentData)
                    if (data != null && data.Name != EnchantmentName.なし && GetMaxLevel(_database, data.Name) > 0 && !_enchantments.Contains(data.Name))
                        _enchantments.Add(data.Name);
            var labels = _enchantments.ConvertAll(name => name.ToString());
            for (int i = 0; i < 3; i++)
            {
                if (!_enchantments.Contains(_enchantmentNames[i])) _enchantmentNames[i] = EnchantmentName.なし;
                _enchantmentFields[i].choices = new List<string>(labels);
                _enchantmentFields[i].SetValueWithoutNotify(_enchantmentNames[i].ToString());
                UpdateLevel(i);
            }
        }

        /// <summary>レベルは1以上で上限なしとし、「なし」の枠だけ入力を無効にする。</summary>
        private void UpdateLevel(int index)
        {
            int max = GetMaxLevel(_database, _enchantmentNames[index]);
            _levels[index] = Mathf.Max(1, _levels[index]);
            _levelFields[index].SetValueWithoutNotify(_levels[index]);
            _levelFields[index].SetEnabled(_enchantmentNames[index] != EnchantmentName.なし);
            _levelFields[index].tooltip = $"1以上を指定できます。効果データはLv{max}まであり、それ以降は最後のレベル間の増加量で効果を延長します。";
        }

        /// <summary>再生中だけ追加を許可し、所持数と高レベル付呪の効果計算について案内する。</summary>
        private void UpdateStatus()
        {
            if (_addButton == null) return;
            bool ready = Application.isPlaying && _inventory != null && _selectedWeapon != null;
            _addButton.SetEnabled(ready);
            _weaponIcon.sprite = _selectedWeapon != null ? _selectedWeapon.weaponIcon : null;
            _contextHelp.text = !Application.isPlaying ? "SelectScene を再生すると所持武器を追加できます。"
                : _inventory == null ? "WeaponInventory があるシーンを再生してください。"
                : $"所持武器: {_inventory.GetAllWeapons().Count} 本。追加した武器は保存され、所持一覧にも反映されます。";
            for (int i = 0; i < 3; i++)
                if (_enchantmentNames[i] != EnchantmentName.なし && _levels[i] > GetMaxLevel(_database, _enchantmentNames[i]))
                {
                    _contextHelp.text += "\n効果データの上限を超える付呪は、最後のレベル間の増加量で効果を延長します。";
                    break;
                }
        }

        /// <summary>GUIの設定から付呪適用済みの武器を作成し、保存と一覧への変更通知まで行う。</summary>
        private void AddSelectedWeapon()
        {
            RefreshContext();
            if (!Application.isPlaying || _inventory == null || _selectedWeapon == null) return;
            try
            {
                var weapon = CreateWeapon(_selectedWeapon, _database, _enchantmentNames, _levels);
                if (!_inventory.AddWeapon(weapon)) return;
                _resultHelp.messageType = HelpBoxMessageType.Info;
                _resultHelp.text = $"「{weapon.DisplayName}」を1本追加しました。";
            }
            catch (Exception exception)
            {
                _resultHelp.messageType = HelpBoxMessageType.Error;
                _resultHelp.text = $"追加できませんでした: {exception.Message}";
                Debug.LogException(exception);
            }
            _resultHelp.RemoveFromClassList("is-hidden");
            UpdateStatus();
        }

        /// <summary>効果配列の末尾の番号を返す。配列の0番目はLv0なので、最大レベルは長さより1小さい。</summary>
        internal static int GetMaxLevel(Enchantment database, EnchantmentName name)
        {
            var data = database != null && database.EnchantmentData != null
                ? database.EnchantmentData.Find(entry => entry != null && entry.Name == name) : null;
            return data?.Efficacy != null ? Mathf.Max(0, data.Efficacy.Length - 1) : 0;
        }

        /// <summary>「なし」の枠を除いて付呪を設定し、追加前に武器のステータスへ効果を反映する。</summary>
        internal static WeaponInstance CreateWeapon(WeaponTemplate template, Enchantment database, EnchantmentName[] names, int[] levels)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            if (names == null || levels == null || names.Length != 3 || levels.Length != 3)
                throw new ArgumentException("付呪は3枠で指定してください。");
            var slots = new List<EnchantmentSlot>();
            for (int i = 0; i < 3; i++)
            {
                if (names[i] == EnchantmentName.なし) continue;
                int max = GetMaxLevel(database, names[i]);
                if (max == 0) throw new InvalidOperationException($"{names[i]}の付呪データがありません。");
                slots.Add(new EnchantmentSlot(names[i], Mathf.Max(1, levels[i])));
            }
            var weapon = new WeaponInstance(template, database);
            // 文字列だけでなくHPや移動速度などの実際の数値も、ここで付呪から再計算する。
            weapon.SetEnchantments(slots.ToArray());
            return weapon;
        }
    }
}
