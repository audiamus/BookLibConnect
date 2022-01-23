using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using core.audiamus.aux;
using core.audiamus.aux.win;
using core.audiamus.connect.ex;
using R = core.audiamus.connect.ui.Properties.Resources;
using static core.audiamus.aux.Logging;

namespace core.audiamus.connect.ui {
  public partial class ManageProfilesForm : Form {
    record ProfileDesc (string Tag, IProfileKeyEx Key, string Alias);

    private readonly AffineSynchronizationContext _sync;
    private List<ProfileDesc> _profiles;
    private bool _ignoreFlag;

    private AudibleClient Client { get; }

    public ManageProfilesForm (AudibleClient client) {
      InitializeComponent ();
      Log (3, this);
      Client = client;
      _sync = new ();
    }

    protected override async void OnLoad (EventArgs e) {
      base.OnLoad (e);
      await loadProfilesAsync ();
    }

    private async Task loadProfilesAsync () {
      using var _ = new LogGuard (3, this);
      using (new ResourceGuard (x => _ignoreFlag = x)) {
        ckBoxEncrypt.Checked = Client.ConfigSettings?.EncryptConfiguration ?? false;

        comBoxProfiles.SelectedIndex = -1;
        comBoxProfiles.Items.Clear ();
      }

      var accountAliases = Client.GetAccountAliases ();
      var profileKeys = await Client.GetProfilesAsync ();

      if (profileKeys?.Any () ?? false) {
        _profiles = new List<ProfileDesc> ();
        foreach (var key in profileKeys) {
          string alias = accountAliases.FirstOrDefault (n => n.AccountId == key.AccountId)?.Alias;
          if (alias is null)
            alias = await Client.GetProfileAliasAsync (key, getAccountAlias);
          string tag = $"{alias}; {key.Region}";
          var p = new ProfileDesc (tag, key, alias);
          _profiles.Add (p);
        }
        comBoxProfiles.Items.AddRange (_profiles.Select (p => p.Tag).ToArray ());

        selectProfile (Client.ProfileKey);
      }
    }

    private void selectProfile (IProfileKey key) {
      int idx = _profiles
        .Select ((p, i) => new { p, i })
        .FirstOrDefault (k => k.p.Key.Matches (key))?.i ?? -1;

      comBoxProfiles.SelectedIndex = idx;
    }

    private void enable () {
      btnRemove.Enabled = comBoxProfiles.SelectedIndex >= 0;
      panelOK.Enabled = btnRemove.Enabled;
    }

    private void comBoxProfiles_SelectedIndexChanged (object sender, EventArgs e) {
      int idx = comBoxProfiles.SelectedIndex;
      if (idx < 0 || idx > _profiles.Count - 1) {
        lblAccount.Text = null;
        lblDevice.Text = null;
      } else {
        var profile = _profiles[idx];
        lblAccount.Text = profile.Key.AccountName;
        lblDevice.Text = profile.Key.DeviceName;
      }
      enable ();
    }

    private async void btnAdd_Click (object sender, EventArgs e) {
      Log (3, this);
      var dlg = new NewProfileForm (Client, _profiles?.Select (p => p.Key).ToList ());
      var result = dlg.ShowDialog ();

      if (result != DialogResult.OK)
        return;

      await loadProfilesAsync ();

      selectProfile (dlg.ProfileKey);

      enable ();
    }

    private async void btnRemove_Click (object sender, EventArgs e) {
      Log (3, this);
      int idx = comBoxProfiles.SelectedIndex;
      if (idx < 0 || idx > _profiles.Count - 1)
        return;

      var profile = _profiles[idx];
      var key = profile.Key;
      string msg = string.Format (R.MsgRemProfileQuestion, key.Region, key.AccountName, key.DeviceName);
      var result =
        MsgBox.Show (this, msg, R.CptRemProfile,
          MessageBoxButtons.YesNo, MessageBoxIcon.Question,
          MessageBoxDefaultButton.Button2);

      if (result != DialogResult.Yes)
        return;

      var removeResult = await Client.RemoveProfileAsync (key);
      if (removeResult >= EAuthorizeResult.succ) {
        if (removeResult == EAuthorizeResult.succ) {
          msg = string.Format (R.MsgRemProfileOk, key.DeviceName);
          MsgBox.Show (this, msg, R.CptRemProfile,
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        } else {
          msg = string.Format (R.MsgRemProfileWarn, key.DeviceName);
          MsgBox.Show (this, msg, R.CptRemProfile,
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        await loadProfilesAsync ();
        enable ();
      } else 
          MsgBox.Show (this, R.MsgRemProfileError, R.CptRemProfile,
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private async void btnOK_Click (object sender, EventArgs e) {
      Log (3, this);
      bool encryptChanged = Client.ConfigSettings is not null && Client.ConfigSettings.EncryptConfiguration != ckBoxEncrypt.Checked;
      if (encryptChanged)
        Client.ConfigSettings.EncryptConfiguration = ckBoxEncrypt.Checked;

      int idx = comBoxProfiles.SelectedIndex;
      if (idx >= 0 && idx < _profiles.Count) {

        // make the currently selected profile the default one 
        var profileKey = _profiles[idx].Key;
        bool? changed = await Client.ChangeProfileAsync (profileKey);

        if (changed ?? false)
          DialogResult = DialogResult.OK;
      }

      Close ();
    }

    private void ckBoxEncrypt_CheckedChanged (object sender, EventArgs e) {
      if (_ignoreFlag)
        return;
      if (ckBoxEncrypt.Checked)
        return;

      DialogResult result = MsgBox.Show (this, R.MsgConfigUnencrypted, this.Text, 
        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

      if (result != DialogResult.Yes) {
        using (new ResourceGuard ( x => _ignoreFlag = x))
          ckBoxEncrypt.Checked = true;
      }

    }

    private bool getAccountAlias (AccountAliasContext ctxt) {
      return _sync.Send(getAccountAliasSync, ctxt);

      static bool getAccountAliasSync (AccountAliasContext ctxt) {
        var dlg = new AccountAliasForm (ctxt);
        dlg.ShowDialog ();
        return true;
      }
    }

  }
}
