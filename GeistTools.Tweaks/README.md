# GeistTools.Tweaks

A collection of Quality-of-Life and bugfixes for Atlyss.
&nbsp;
# Current Tweaks

<details>
	<summary>UI Scaling Fix / Ultrawide Fix</summary>
	
	## UI Scaling Fix / Ultrawide Fix
	Fixes inconsistent scaling in Atlyss, giving UI a more crisp look, and making the UI less cluttered.

	Equivalent of an ultrawide fix, but will result in nicer-looking UI elements all-around.

	### Config [Tweaks.UIScalingFix]

	- Enabled
		- Desc: Whether or not the fixes are applied
		- Type: Boolean
		- Default: `true`
	- ScaleRatio
		- Desc: The ratio of scaling between width and height on UI canvases. Best to keep to 1 all-around.
		- Type: float
		- Default: 1
	- UIResolutionWidth
		- Desc: The equivalent resolution width to render the UI at. (Increasing has diminishing returns, change wisely.)
		- Type: int
		- Default: 1600
	- UIResolutionHeight
		- Desc: The equivalent resolution width to render the UI at. (Increasing has diminishing returns, change wisely.)
		- Type: float
		- Default: 900
	- EscapeMenuOverride
		- Desc: Specific overall scaling for the Escape/Pause Menu.
		- Type: float
		- Default: 1.5
</details>
&nbsp;

# Planned Tweaks

- [x] UI Scaling Fix / Ultrawide Fix
- [ ] Training Dummy DPS Meter
- [ ] Auto-Select Last Character
- [ ] Easier Ledge Grabbing
- [ ] Shop Sell/Buy Quantity Max Button

