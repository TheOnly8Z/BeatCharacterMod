using BaseLib.Utils;
using BeatCharacterMod.BeatCharacterModCode.Enums;
using BeatCharacterMod.BeatCharacterModCode.Extensions;
using BeatCharacterMod.BeatCharacterModCode.Singletons;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace BeatCharacterMod.BeatCharacterModCode.Nodes;

public partial class NTempoCounter : Control
{

	private static readonly StringName _v = new("v");

	private static readonly StringName _s = new("s");

	private static readonly string _starGainVfxPath = SceneHelper.GetScenePath("vfx/star_gain_vfx");

	private Player? _player;

	// private MegaRichTextLabel _label;
	private RichTextLabel _label;
	
	private Control _icon;

	private ShaderMaterial _hsv;

	private float _lerpingStarCount;

	private float _velocity;

	private int _displayedStarCount;

	private Tween? _hsvTween;

	private bool _isListeningToCombatState;

	private HoverTip _hoverTip;
	
	public override void _Ready()
	{
		_label = GetNode<RichTextLabel>("%CountLabel");
		_icon = GetNode<Control>("Icon");
		_hsv = (ShaderMaterial)_icon.Material;
		LocString locString = new LocString("static_hover_tips", "TEMPO_COUNT.description");
		locString.Add("tempoIcon", "[img]res://BeatCharacterMod/images/packed/sprite_fonts/tempo_icon.png[/img]");
		_hoverTip = new HoverTip(new LocString("static_hover_tips", "TEMPO_COUNT.title"), locString);
		Connect(Control.SignalName.MouseEntered, Callable.From(OnHovered));
		Connect(Control.SignalName.MouseExited, Callable.From(OnUnhovered));
		base.Visible = false;
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		ConnectTempoChangedSignal();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (_player != null && _isListeningToCombatState)
		{
			_player.PlayerCombatState.MelodicFlow().TempoChanged -= OnTempoChanged;
			_isListeningToCombatState = false;
		}
	}

	private void ConnectTempoChangedSignal()
	{
		if (_player != null && !_isListeningToCombatState)
		{
			_player.PlayerCombatState.MelodicFlow().TempoChanged += OnTempoChanged;
			_isListeningToCombatState = true;
		}
	}

	public void Initialize(Player player)
	{
		_player = player;
		ConnectTempoChangedSignal();
		RefreshVisibility();
	}

	private void OnHovered()
	{
		NHoverTipSet.CreateAndShow(this, _hoverTip)?.SetGlobalPosition(base.GlobalPosition + new Vector2(-34f, -300f));
	}

	private void OnUnhovered()
	{
		NHoverTipSet.Remove(this);
	}

	private void OnTempoChanged(int oldStars, int newStars)
	{
		UpdateStarCount(oldStars, newStars);
		RefreshVisibility();
	}

	public override void _Process(double delta)
	{
		if (_player != null)
		{
			/*
			float num = ((_player.PlayerCombatState.Stars == 0) ? 5f : 30f);
			for (int i = 0; i < _rotationLayers.GetChildCount(); i++)
			{
				_rotationLayers.GetChild<Control>(i).RotationDegrees += (float)delta * num * (float)(i + 1);
			}
			*/
			_lerpingStarCount = MathHelper.SmoothDamp(_lerpingStarCount, (int)MelodicFlowTracker.GetTempo(_player), ref _velocity, 0.1f, (float)delta);
			SetStarCountText(Mathf.RoundToInt(_lerpingStarCount));
		}
	}

	private void UpdateStarCount(int oldCount, int newCount)
	{
		if (newCount < oldCount)
		{
			_hsvTween?.Kill();
			_hsv.SetShaderParameter(_v, 1f);
			_lerpingStarCount = newCount;
			SetStarCountText(newCount);
		}
		else if (newCount > oldCount)
		{
			_hsvTween?.Kill();
			_hsvTween = CreateTween();
			_hsvTween.TweenMethod(Callable.From<float>(UpdateShaderV), 2f, 1f, 0.20000000298023224);
			Node2D node2D = PreloadManager.Cache.GetAsset<PackedScene>(_starGainVfxPath).Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
			this.AddChildSafely(node2D);
			this.MoveChildSafely(node2D, 0);
			node2D.Position = base.Size / 2f;
		}
	}

	private void SetStarCountText(int tempo)
	{
		if (_displayedStarCount != tempo)
		{
			_displayedStarCount = tempo;
			_label.AddThemeColorOverride(ThemeConstants.Label.FontColor, (tempo == 0) ? StsColors.red : StsColors.cream);
			_label.Text = $"[center]{tempo}[/center]";
			if (tempo == 0)
			{
				_hsv.SetShaderParameter(_s, 0.5f);
				_hsv.SetShaderParameter(_v, 0.85f);
			}
			else
			{
				_hsv.SetShaderParameter(_s, 1f);
				_hsv.SetShaderParameter(_v, 1f);
			}
		}
	}

	private void UpdateShaderV(float value)
	{
		_hsv.SetShaderParameter(_v, value);
	}

	private void RefreshVisibility()
	{
		if (_player == null)
		{
			base.Visible = false;
			return;
		}
		base.Visible = base.Visible
		               || _player.PlayerCombatState.MelodicFlow().MelodicState != MelodicState.None
		               || _player.PlayerCombatState.MelodicFlow().Tempo > 0;
	}
}