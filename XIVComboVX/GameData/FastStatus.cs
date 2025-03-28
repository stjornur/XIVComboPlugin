using System.Diagnostics.CodeAnalysis;

using Dalamud.Game.ClientState.Objects.Types;

using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;

using Lumina.Excel;

using LuminaStatus = Lumina.Excel.Sheets.Status;
using CsStatus = FFXIVClientStructs.FFXIV.Client.Game.Status;

namespace PrincessRTFM.XIVComboVX.GameData;

[SuppressMessage("ReSharper", "RemoveRedundantBraces")]
[SuppressMessage("ReSharper", "SuggestVarOrType_Elsewhere")]
[SuppressMessage("ReSharper", "SuggestVarOrType_BuiltInTypes")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("Style", "IDE0008:Use explicit type")]
public unsafe class Status(CsStatus* statusPtr) {
	public CsStatus* Struct => statusPtr;
	public ushort StatusId => this.Struct->StatusId;
	public ushort Param => this.Struct->Param;
	public float RemainingTime => this.Struct->RemainingTime;
	public uint SourceId => this.Struct->SourceId;

	public RowRef<LuminaStatus> GameData => new(Service.GameData.Excel, this.StatusId);

	public static Status? GetStatus(IBattleChara battleChara, uint statusId) {
		var sm = &((BattleChara*)battleChara.Address)->StatusManager;
		var spanSize = sm->NumValidStatuses;

		for (int i = 0; i < spanSize; i++) {
			var status = sm->Status.GetPointer(i);
			if (status->StatusId == statusId) {
				return new Status(status);
			}
		}

		return null;
	}

	public static Status? GetStatus(IBattleChara battleChara, uint statusId, uint sourceId) {
		var sm = &((BattleChara*)battleChara.Address)->StatusManager;

		for (int i = 0; i < sm->NumValidStatuses; i++) {
			var status = sm->Status.GetPointer(i);
			if (status->StatusId == statusId &&
			    (status->SourceId == sourceId || status->SourceId is 0xE000_0000 or 0)) {
				return new Status(status);
			}
		}

		return null;
	}
}
