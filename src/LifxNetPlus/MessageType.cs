namespace LifxNetPlus {
	/// <summary>
	///     The message type used for LAN communication
	/// </summary>
	public enum MessageType : ushort {
		//Device Messages
		/// <summary>
		/// The device set site message type
		/// </summary>
		DeviceSetSite = 1,
		/// <summary>
		/// The device get service message type
		/// </summary>
		DeviceGetService = 2,
		/// <summary>
		/// The device state service message type
		/// </summary>
		DeviceStateService = 3,

		/// <summary>
		/// The device get time message type
		/// </summary>
		DeviceGetTime = 4,
		/// <summary>
		/// The device set time message type
		/// </summary>
		DeviceSetTime = 5,
		/// <summary>
		/// The device state time message type
		/// </summary>
		DeviceStateTime = 6,
		/// <summary>
		/// The device get reset switch message type
		/// </summary>
		DeviceGetResetSwitch = 7,
		/// <summary>
		/// The device state reset switch message type
		/// </summary>
		DeviceStateResetSwitch = 8,
		/// <summary>
		/// The device get dummy load message type
		/// </summary>
		DeviceGetDummyLoad = 9,
		/// <summary>
		/// The device set dummy load message type
		/// </summary>
		DeviceSetDummyLoad = 10,
		/// <summary>
		/// The device state dummy load message type
		/// </summary>
		DeviceStateDummyLoad = 11,

		/// <summary>
		/// The device get host info message type
		/// </summary>
		DeviceGetHostInfo = 12,
		/// <summary>
		/// The device state host info message type
		/// </summary>
		DeviceStateHostInfo = 13,
		/// <summary>
		/// The device get host firmware message type
		/// </summary>
		DeviceGetHostFirmware = 14,
		/// <summary>
		/// The device state host firmware message type
		/// </summary>
		DeviceStateHostFirmware = 15,
		/// <summary>
		/// The device get wifi info message type
		/// </summary>
		DeviceGetWifiInfo = 16,
		/// <summary>
		/// The device state wifi info message type
		/// </summary>
		DeviceStateWifiInfo = 17,
		/// <summary>
		/// The device get wifi firmware message type
		/// </summary>
		DeviceGetWifiFirmware = 18,
		/// <summary>
		/// The device state wifi firmware message type
		/// </summary>
		DeviceStateWifiFirmware = 19,
		/// <summary>
		/// The device get power message type
		/// </summary>
		DeviceGetPower = 20,
		/// <summary>
		/// The device set power message type
		/// </summary>
		DeviceSetPower = 21,
		/// <summary>
		/// The device state power message type
		/// </summary>
		DeviceStatePower = 22,
		/// <summary>
		/// The device get label message type
		/// </summary>
		DeviceGetLabel = 23,
		/// <summary>
		/// The device set label message type
		/// </summary>
		DeviceSetLabel = 24,
		/// <summary>
		/// The device state label message type
		/// </summary>
		DeviceStateLabel = 25,
		/// <summary>
		/// The device get tags message type
		/// </summary>
		DeviceGetTags = 26,
		/// <summary>
		/// The device set tags message type
		/// </summary>
		DeviceSetTags = 27,
		/// <summary>
		/// The device state tags message type
		/// </summary>
		DeviceStateTags = 28,
		/// <summary>
		/// The device get tag labels message type
		/// </summary>
		DeviceGetTagLabels = 29,
		/// <summary>
		/// The device set tag labels message type
		/// </summary>
		DeviceSetTagLabels = 30,
		/// <summary>
		/// The device state tag labels message type
		/// </summary>
		DeviceStateTagLabels = 31,
		/// <summary>
		/// The device get version message type
		/// </summary>
		DeviceGetVersion = 32,
		/// <summary>
		/// The device state version message type
		/// </summary>
		DeviceStateVersion = 33,
		/// <summary>
		/// The device get info message type
		/// </summary>
		DeviceGetInfo = 34,
		/// <summary>
		/// The device state info message type
		/// </summary>
		DeviceStateInfo = 35,
		/// <summary>
		/// The device get mcu rail voltage message type
		/// </summary>
		DeviceGetMcuRailVoltage = 36,
		/// <summary>
		/// The device state mcu rail voltage message type
		/// </summary>
		DeviceStateMcuRailVoltage = 37,
		/// <summary>
		/// The device set reboot message type
		/// </summary>
		DeviceSetReboot = 38,
		/// <summary>
		/// The device disable factory test mode message type
		/// </summary>
		DeviceDisableFactoryTestMode = 40,
		/// <summary>
		/// The device state factory test mode message type
		/// </summary>
		DeviceStateFactoryTestMode = 41,
		/// <summary>
		/// The device state site message type
		/// </summary>
		DeviceStateSite = 42,
		/// <summary>
		/// The device state reboot message type
		/// </summary>
		DeviceStateReboot = 43,
		/// <summary>
		/// The device set pan gateway message type
		/// </summary>
		DeviceSetPanGateway = 44,
		/// <summary>
		/// The device acknowledgement message type
		/// </summary>
		DeviceAcknowledgement = 45,
		/// <summary>
		/// The device set factory reset message type
		/// </summary>
		DeviceSetFactoryReset = 46,
		/// <summary>
		/// The device state factory reset message type
		/// </summary>
		DeviceStateFactoryReset = 47,
		/// <summary>
		/// The device get location message type
		/// </summary>
		DeviceGetLocation = 48,
		/// <summary>
		/// The device set location message type
		/// </summary>
		DeviceSetLocation = 49,
		/// <summary>
		/// The device state location message type
		/// </summary>
		DeviceStateLocation = 50,
		/// <summary>
		/// The device get group message type
		/// </summary>
		DeviceGetGroup = 51,
		/// <summary>
		/// The device set group message type
		/// </summary>
		DeviceSetGroup = 52,
		/// <summary>
		/// The device state group message type
		/// </summary>
		DeviceStateGroup = 53,
		/// <summary>
		/// The device get owner message type
		/// </summary>
		DeviceGetOwner = 54,
		/// <summary>
		/// The device set owner message type
		/// </summary>
		DeviceSetOwner = 55,
		/// <summary>
		/// The device state owner message type
		/// </summary>
		DeviceStateOwner = 56,
		/// <summary>
		/// The device echo request message type
		/// </summary>
		DeviceEchoRequest = 58,
		/// <summary>
		/// The device echo response message type
		/// </summary>
		DeviceEchoResponse = 59,
		/// <summary>
		/// The device get onboarding feedback message type
		/// </summary>
		DeviceGetOnboardingFeedback = 96,
		/// <summary>
		/// The device set onboarding feedback message type
		/// </summary>
		DeviceSetOnboardingFeedback = 97,
		/// <summary>
		/// The device clear onboarding feedback message type
		/// </summary>
		DeviceClearOnboardingFeedback = 98,

		//Light messages
		/// <summary>
		/// The light get message type
		/// </summary>
		LightGet = 101,
		/// <summary>
		/// The light set color message type
		/// </summary>
		LightSetColor = 102,
		/// <summary>
		/// The light set waveform message type
		/// </summary>
		LightSetWaveform = 103,
		/// <summary>
		/// The set light brightness message type
		/// </summary>
		SetLightBrightness = 104,
		/// <summary>
		/// The light set dim relative message type
		/// </summary>
		LightSetDimRelative = 105,
		/// <summary>
		/// The light set rgbw message type
		/// </summary>
		LightSetRgbw = 106,
		/// <summary>
		/// The light state message type
		/// </summary>
		LightState = 107,
		/// <summary>
		/// The light get rail voltage message type
		/// </summary>
		LightGetRailVoltage = 108,
		/// <summary>
		/// The light set rail voltage message type
		/// </summary>
		LightSetRailVoltage = 109,
		/// <summary>
		/// The light get temperature message type
		/// </summary>
		LightGetTemperature = 110,
		/// <summary>
		/// The light state temperature message type
		/// </summary>
		LightStateTemperature = 111,
		/// <summary>
		/// The light get power message type
		/// </summary>
		LightGetPower = 116,
		/// <summary>
		/// The light set power message type
		/// </summary>
		LightSetPower = 117,
		/// <summary>
		/// The light state power message type
		/// </summary>
		LightStatePower = 118,
		/// <summary>
		/// The light set waveform optional message type
		/// </summary>
		LightSetWaveformOptional = 119,

		//Infrared
		/// <summary>
		/// The light get infrared message type
		/// </summary>
		LightGetInfrared = 120,
		/// <summary>
		/// The light state infrared message type
		/// </summary>
		LightStateInfrared = 121,
		/// <summary>
		/// The light set infrared message type
		/// </summary>
		LightSetInfrared = 122,
		/// <summary>
		/// The light get light restore message type
		/// </summary>
		LightGetLightRestore = 129,
		/// <summary>
		/// The light set light restore message type
		/// </summary>
		LightSetLightRestore = 130,
		/// <summary>
		/// The light state light restore message type
		/// </summary>
		LightStateLightRestore = 131,
		/// <summary>
		/// The light set smooth message type
		/// </summary>
		LightSetSmooth = 141,
		/// <summary>
		/// The light get hev cycle message type
		/// </summary>
		LightGetHevCycle = 142,
		/// <summary>
		/// The light set hev cycle message type
		/// </summary>
		LightSetHevCycle = 143,
		/// <summary>
		/// The light state hev cycle message type
		/// </summary>
		LightStateHevCycle = 144,
		/// <summary>
		/// The light get hev cycle configuration message type
		/// </summary>
		LightGetHevCycleConfiguration = 145,
		/// <summary>
		/// The light set hev cycle configuration message type
		/// </summary>
		LightSetHevCycleConfiguration = 146,
		/// <summary>
		/// The light state hev cycle configuration message type
		/// </summary>
		LightStateHevCycleConfiguration = 147,
		/// <summary>
		/// The light get last hev cycle result message type
		/// </summary>
		LightGetLastHevCycleResult = 148,
		/// <summary>
		/// The light state last hev cycle result message type
		/// </summary>
		LightStateLastHevCycleResult = 150,

		// WAN
		/// <summary>
		/// The wan get message type
		/// </summary>
		WanGet = 201,
		/// <summary>
		/// The wan set message type
		/// </summary>
		WanSet = 202,
		/// <summary>
		/// The wan state message type
		/// </summary>
		WanState = 203,
		/// <summary>
		/// The wan get auth key message type
		/// </summary>
		WanGetAuthKey = 204,
		/// <summary>
		/// The wan set auth key message type
		/// </summary>
		WanSetAuthKey = 205,
		/// <summary>
		/// The wan state auth key message type
		/// </summary>
		WanStateAuthKey = 206,
		/// <summary>
		/// The wan set keep alive message type
		/// </summary>
		WanSetKeepAlive = 207,
		/// <summary>
		/// The wan state keep alive message type
		/// </summary>
		WanStateKeepAlive = 208,
		/// <summary>
		/// The wan set host message type
		/// </summary>
		WanSetHost = 209,
		/// <summary>
		/// The wan get host message type
		/// </summary>
		WanGetHost = 210,
		/// <summary>
		/// The wan state host message type
		/// </summary>
		WanStateHost = 211,
		/// <summary>
		/// The wan get cloud message type
		/// </summary>
		WanGetCloud = 212,
		/// <summary>
		/// The wan set cloud message type
		/// </summary>
		WanSetCloud = 213,
		/// <summary>
		/// The wan state cloud message type
		/// </summary>
		WanStateCloud = 214,

		// DevState?
		/// <summary>
		/// The device state unhandled message type
		/// </summary>
		DeviceStateUnhandled = 223,
		/// <summary>
		/// The device get grouping message type
		/// </summary>
		DeviceGetGrouping = 224,
		/// <summary>
		/// The device set grouping message type
		/// </summary>
		DeviceSetGrouping = 225,
		/// <summary>
		/// The device state grouping message type
		/// </summary>
		DeviceStateGrouping = 226,

		//Wifi
		/// <summary>
		/// The wifi get message type
		/// </summary>
		WifiGet = 301,
		/// <summary>
		/// The wifi set message type
		/// </summary>
		WifiSet = 302,
		/// <summary>
		/// The wifi state message type
		/// </summary>
		WifiState = 303,
		/// <summary>
		/// The wifi get access points message type
		/// </summary>
		WifiGetAccessPoints = 304,
		/// <summary>
		/// The wifi set access points message type
		/// </summary>
		WifiSetAccessPoints = 305,
		/// <summary>
		/// The wifi state access points message type
		/// </summary>
		WifiStateAccessPoints = 306,
		/// <summary>
		/// The wifi get access point message type
		/// </summary>
		WifiGetAccessPoint = 307,
		/// <summary>
		/// The wifi state access point message type
		/// </summary>
		WifiStateAccessPoint = 308,
		/// <summary>
		/// The wifi set access point broadcast message type
		/// </summary>
		WifiSetAccessPointBroadcast = 309,

		//Sensor
		/// <summary>
		/// The sensor get ambient light message type
		/// </summary>
		SensorGetAmbientLight = 401,
		/// <summary>
		/// The sensor state ambient light message type
		/// </summary>
		SensorStateAmbientLight = 402,
		/// <summary>
		/// The sensor get dimmer voltage message type
		/// </summary>
		SensorGetDimmerVoltage = 403,
		/// <summary>
		/// The sensor state dimmer voltage message type
		/// </summary>
		SensorStateDimmerVoltage = 404,
		/// <summary>
		/// The sensor get power instantaneous message type
		/// </summary>
		SensorGetPowerInstantaneous = 405,
		/// <summary>
		/// The sensor state power instantaneous message type
		/// </summary>
		SensorStatePowerInstantaneous = 406,

		//Multi zone
		/// <summary>
		/// The set color zones message type
		/// </summary>
		SetColorZones = 501,
		/// <summary>
		/// The get color zones message type
		/// </summary>
		GetColorZones = 502,
		/// <summary>
		/// The state zone message type
		/// </summary>
		StateZone = 503,
		/// <summary>
		/// The state multi zone message type
		/// </summary>
		StateMultiZone = 506,
		/// <summary>
		/// The multi zone get effect message type
		/// </summary>
		MultiZoneGetEffect = 507,
		/// <summary>
		/// The multi zone set effect message type
		/// </summary>
		MultiZoneSetEffect = 508,
		/// <summary>
		/// The multizone state effect message type
		/// </summary>
		MultizoneStateEffect = 509,
		/// <summary>
		/// The set extended color zones message type
		/// </summary>
		SetExtendedColorZones = 510,
		/// <summary>
		/// The get extended color zones message type
		/// </summary>
		GetExtendedColorZones = 511,
		/// <summary>
		/// The state extended color zones message type
		/// </summary>
		StateExtendedColorZones = 512,

		//Boo, homekit
		/// <summary>
		/// The homekit get pairing code message type
		/// </summary>
		HomekitGetPairingCode = 601,
		/// <summary>
		/// The homekit state pairing code message type
		/// </summary>
		HomekitStatePairingCode = 602,
		/// <summary>
		/// The homekit get pairing status message type
		/// </summary>
		HomekitGetPairingStatus = 603,
		/// <summary>
		/// The homekit state pairing status message type
		/// </summary>
		HomekitStatePairingStatus = 604,

		//Tile
		/// <summary>
		/// The get device chain message type
		/// </summary>
		GetDeviceChain = 701,
		/// <summary>
		/// The state device chain message type
		/// </summary>
		StateDeviceChain = 702,
		/// <summary>
		/// The set user position message type
		/// </summary>
		SetUserPosition = 703,
		/// <summary>
		/// The get tile state message type
		/// </summary>
		GetTileState1 = 704,
		/// <summary>
		/// The get tile state message type
		/// </summary>
		GetTileState4 = 705,
		/// <summary>
		/// The get tile state 16 message type
		/// </summary>
		GetTileState16 = 706,
		/// <summary>
		/// The get tile state 64 message type
		/// </summary>
		GetTileState64 = 707,
		/// <summary>
		/// The state tile state message type
		/// </summary>
		StateTileState1 = 708,
		/// <summary>
		/// The state tile state message type
		/// </summary>
		StateTileState4 = 709,
		/// <summary>
		/// The state tile state 16 message type
		/// </summary>
		StateTileState16 = 710,
		/// <summary>
		/// The state tile state 64 message type
		/// </summary>
		StateTileState64 = 711,
		/// <summary>
		/// The set tile state message type
		/// </summary>
		SetTileState1 = 712,
		/// <summary>
		/// The set tile state message type
		/// </summary>
		SetTileState4 = 713,
		/// <summary>
		/// The set tile state 16 message type
		/// </summary>
		SetTileState16 = 714,
		/// <summary>
		/// The set tile state 64 message type
		/// </summary>
		SetTileState64 = 715,
		/// <summary>
		/// The tile copy frame buffer message type
		/// </summary>
		TileCopyFrameBuffer = 716,
		/// <summary>
		/// The tile fill frame buffer message type
		/// </summary>
		TileFillFrameBuffer = 717,
		/// <summary>
		/// The tile get effect message type
		/// </summary>
		TileGetEffect = 718,
		/// <summary>
		/// The tile set effect message type
		/// </summary>
		tileSetEffect = 719,
		/// <summary>
		/// The tile state effect message type
		/// </summary>
		TileStateEffect = 720,
		/// <summary>
		/// The get tile tap config message type
		/// </summary>
		GetTileTapConfig = 721,
		/// <summary>
		/// The set tile tap config message type
		/// </summary>
		SetTileTapConfig = 722,
		/// <summary>
		/// The state tile tap config message type
		/// </summary>
		StateTileTapConfig = 723,

		//Switch 
		/// <summary>
		/// The get relay mode message type
		/// </summary>
		GetRelayMode = 800,
		/// <summary>
		/// The set relay mode message type
		/// </summary>
		SetRelayMode = 801,
		/// <summary>
		/// The state relay mode message type
		/// </summary>
		StateRelayMode = 802,
		/// <summary>
		/// The get all relay modes message type
		/// </summary>
		GetAllRelayModes = 803,
		/// <summary>
		/// The state all relay modes message type
		/// </summary>
		StateAllRelayModes = 804,
		/// <summary>
		/// The get relay label message type
		/// </summary>
		GetRelayLabel = 810,
		/// <summary>
		/// The set relay label message type
		/// </summary>
		SetRelayLabel = 811,
		/// <summary>
		/// The state relay label message type
		/// </summary>
		StateRelayLabel = 812,
		/// <summary>
		/// The get relay group message type
		/// </summary>
		GetRelayGroup = 813,
		/// <summary>
		/// The set relay group message type
		/// </summary>
		SetRelayGroup = 814,
		/// <summary>
		/// The state relay group message type
		/// </summary>
		StateRelayGroup = 815,
		/// <summary>
		/// The get relay power message type
		/// </summary>
		GetRelayPower = 816,
		/// <summary>
		/// The set relay power message type
		/// </summary>
		SetRelayPower = 817,
		/// <summary>
		/// The state relay power message type
		/// </summary>
		StateRelayPower = 818,
		/// <summary>
		/// The get relay location message type
		/// </summary>
		GetRelayLocation = 819,
		/// <summary>
		/// The set relay location message type
		/// </summary>
		SetRelayLocation = 820,
		/// <summary>
		/// The state relay location message type
		/// </summary>
		StateRelayLocation = 821,
		/// <summary>
		/// The get relay message type
		/// </summary>
		GetRelay = 822,
		/// <summary>
		/// The state relay message type
		/// </summary>
		StateRelay = 823,
		/// <summary>
		/// The get button count message type
		/// </summary>
		GetButtonCount = 901,
		/// <summary>
		/// The state button count message type
		/// </summary>
		StateButtonCount = 902
	}
}