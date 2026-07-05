using BaseLib.Utils;
using BeatCharacterMod.BeatCharacterModCode.Enums;
using BeatCharacterMod.BeatCharacterModCode.Extensions;
using BeatCharacterMod.BeatCharacterModCode.Singletons;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
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
	private RichTextLabel _labelStance;
	private RichTextLabel _labelLastCard;

	private Control _icon;

	private ShaderMaterial _hsv;

	private float _lerpingStarCount;

	private float _velocity;

	private int _displayedStarCount;
	private MelodicState _displayedMelodicState;
	private CardType _lastCardType;
	
	private Tween? _hsvTween;

	private bool _isListeningToCombatState;

	private HoverTip? _hoverTip;

	private HoverTip? _hoverTipRhythm;
	private HoverTip? _hoverTipResonance;
	private HoverTip? _hoverTipSilence;
	
	public override void _Ready()
	{
		_label = GetNode<RichTextLabel>("%CountLabel");
		_labelStance = GetNode<RichTextLabel>("%StanceLabel");
		_labelLastCard = GetNode<RichTextLabel>("%LastPlayedTypeLabel");
		_icon = GetNode<Control>("Icon");
		_hsv = (ShaderMaterial)_icon.Material;
		
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
		if (_player?.PlayerCombatState != null && _isListeningToCombatState)
		{
			_player.PlayerCombatState.MelodicFlow().TempoChanged -= OnTempoChanged;
			_player.PlayerCombatState.MelodicFlow().MelodicStateChanged -= OnMelodicStateChanged;
			_player.PlayerCombatState.MelodicFlow().LastPlayedCardTypeChanged -= OnLastPlayedCardTypeChanged;
			_isListeningToCombatState = false;
		}
	}

	private void ConnectTempoChangedSignal()
	{
		if (_player?.PlayerCombatState != null && !_isListeningToCombatState)
		{
			_player.PlayerCombatState.MelodicFlow().TempoChanged += OnTempoChanged;
			_player.PlayerCombatState.MelodicFlow().MelodicStateChanged += OnMelodicStateChanged;
			_player.PlayerCombatState.MelodicFlow().LastPlayedCardTypeChanged += OnLastPlayedCardTypeChanged;
			_isListeningToCombatState = true;
		}
	}

	public void Initialize(Player player)
	{
		_player = player;
		
		LocString locString = new LocString("static_hover_tips", "TEMPO_COUNT.description");
		locString.Add("tempoIcon", "[img]res://BeatCharacterMod/images/packed/sprite_fonts/tempo_icon.png[/img]");
		_hoverTip = new HoverTip(new LocString("static_hover_tips", "TEMPO_COUNT.title"), locString);
		_hoverTipRhythm = new HoverTip(new LocString("static_hover_tips", "MELODIC_FLOW_RHYTHM.title"), new LocString("static_hover_tips", "MELODIC_FLOW_RHYTHM.description"));
		_hoverTipResonance = new HoverTip(new LocString("static_hover_tips", "MELODIC_FLOW_RESONANCE.title"), new LocString("static_hover_tips", "MELODIC_FLOW_RESONANCE.description"));

		LocString locStringSilence = new LocString("static_hover_tips", "MELODIC_FLOW_SILENCE.description");
		locStringSilence.Add("energyPrefix", EnergyIconHelper.GetPrefix(_player.Character.CardPool));
		locStringSilence.Add("tempoIcon", "[img]res://BeatCharacterMod/images/packed/sprite_fonts/tempo_icon.png[/img]");
		_hoverTipSilence = new HoverTip(new LocString("static_hover_tips", "MELODIC_FLOW_SILENCE.title"), locStringSilence);

		
		ConnectTempoChangedSignal();
		RefreshVisibility();
	}

	private void OnHovered()
	{
		if (_hoverTip == null)
		{
			return;
		}
		List<IHoverTip> HoverTips =
		[
			_hoverTip
		];
		if (_player != null && _isListeningToCombatState)
		{
			switch (MelodicFlowTracker.GetMelodicFlowState(_player))
			{
				case MelodicState.Rhythm:
					HoverTips.Add(_hoverTipRhythm);
					break;
				case MelodicState.Resonance:
					HoverTips.Add(_hoverTipResonance);
					break;
				case MelodicState.Silence:
					HoverTips.Add(_hoverTipSilence);
					break;
			}
		}
		NHoverTipSet.CreateAndShow(this, HoverTips)?.SetGlobalPosition(base.GlobalPosition + new Vector2(-34f, -550f));
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

	private void OnMelodicStateChanged(MelodicState oldState, MelodicState newState)
	{
		SetMelodicStateText(newState);
		RefreshVisibility();
	}
	
	private void OnLastPlayedCardTypeChanged(CardType oldType,  CardType newType)
	{
		SetLastPlayedCardText(newType);
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
			SetTempoCountText(Mathf.RoundToInt(_lerpingStarCount));
		}
	}

	private void UpdateStarCount(int oldCount, int newCount)
	{
		if (newCount < oldCount)
		{
			_hsvTween?.Kill();
			_hsv.SetShaderParameter(_v, 1f);
			_lerpingStarCount = newCount;
			SetTempoCountText(newCount);
		}
		else if (newCount > oldCount)
		{
			_hsvTween?.Kill();
			_hsvTween = CreateTween();
			_hsvTween.TweenMethod(Callable.From<float>(UpdateShaderV), 2f, 1f, 0.2);
			Node2D node2D = PreloadManager.Cache.GetAsset<PackedScene>(_starGainVfxPath).Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
			this.AddChildSafely(node2D);
			this.MoveChildSafely(node2D, 0);
			node2D.Position = base.Size / 2f;
		}
	}

	private void SetTempoCountText(int tempo)
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

	private void SetMelodicStateText(MelodicState state)
	{
		if (_displayedMelodicState != state)
		{
			_displayedMelodicState = state;
			_labelStance.Text = new LocString("static_hover_tips", "MELODIC_FLOW_" + _displayedMelodicState.ToString().ToUpper() + ".title").GetRawText();
		}
	}

	private void SetLastPlayedCardText(CardType type)
	{
		if (_lastCardType != type)
		{
			_lastCardType = type;
			_labelLastCard.Text = type.ToLocString().GetRawText();
		}
	}

	private void UpdateShaderV(float value)
	{
		_hsv.SetShaderParameter(_v, value);
	}

	private void RefreshVisibility()
	{
		if (_player?.PlayerCombatState == null)
		{
			base.Visible = false;
			return;
		}
		base.Visible = base.Visible
		               || _player.PlayerCombatState.MelodicFlow().MelodicState != MelodicState.None
		               || _player.PlayerCombatState.MelodicFlow().Tempo > 0;
	}
}