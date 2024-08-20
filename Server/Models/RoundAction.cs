using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Server.Models;

public enum RoundActionType
{
    SendEmployeeForTraining,
    ParticipateInCallForTenders,
    RecruitAConsultant,
    FireAnEmployee,
    PassMyTurn
}

[JsonDerivedType(typeof(RoundAction), typeDiscriminator: "DEFAULT")]
[JsonDerivedType(typeof(SendEmployeeForTrainingRoundAction), typeDiscriminator: "SendEmployeeForTraining")]
[JsonDerivedType(typeof(ParticipateInCallForTendersRoundAction), typeDiscriminator: "ParticipateInCallForTenders")]
[JsonDerivedType(typeof(RecruitAConsultantRoundAction), typeDiscriminator: "RecruitAConsultant")]
[JsonDerivedType(typeof(FireAnEmployeeRoundAction), typeDiscriminator: "FireAnEmployee")]
[JsonDerivedType(typeof(PassMyTurnRoundAction), typeDiscriminator: "PassMyTurn")]
public class RoundAction(int playerId)
{
    public static RoundAction CreateForType(RoundActionType actionType, int playerId, JsonObject payload)
    {
        RoundAction action = actionType switch
        {
            RoundActionType.SendEmployeeForTraining => new SendEmployeeForTrainingRoundAction(playerId),
            RoundActionType.ParticipateInCallForTenders => new ParticipateInCallForTendersRoundAction(playerId),
            RoundActionType.RecruitAConsultant => new RecruitAConsultantRoundAction(playerId),
            RoundActionType.FireAnEmployee => new FireAnEmployeeRoundAction(playerId),
            RoundActionType.PassMyTurn => new PassMyTurnRoundAction(playerId),
            _ => new PassMyTurnRoundAction(playerId),
        };

        action.ApplyPayload(payload);

        return action;
    }

    protected virtual void ApplyPayload(JsonObject payload) { }

    public int PlayerId { get; init; } = playerId;
}

public class SendEmployeeForTrainingRoundAction(int playerId) : RoundAction(playerId)
{
    public class SendEmployeeForTrainingPayload
    {
        public int EmployeeId { get; init; }
    }

    public SendEmployeeForTrainingPayload Payload { get; private set; } = null!;

    protected override void ApplyPayload(JsonObject payload)
    {
        Payload = JsonSerializer.Deserialize<SendEmployeeForTrainingPayload>(payload)!;
    }
}

public class ParticipateInCallForTendersRoundAction(int playerId) : RoundAction(playerId)
{
    public class ParticipateInCallForTendersPayload
    {
        public int CallForTendersId { get; init; }
    }

    public ParticipateInCallForTendersPayload Payload { get; private set; } = null!;

    protected override void ApplyPayload(JsonObject payload)
    {
        Payload = JsonSerializer.Deserialize<ParticipateInCallForTendersPayload>(payload)!;
    }
}

public class RecruitAConsultantRoundAction(int playerId) : RoundAction(playerId)
{
    public class RecruitAConsultantPayload
    {
        public int ConsultantId { get; init; }
    }

    public RecruitAConsultantPayload Payload { get; private set; } = null!;

    protected override void ApplyPayload(JsonObject payload)
    {
        Payload = JsonSerializer.Deserialize<RecruitAConsultantPayload>(payload)!;
    }
}

public class FireAnEmployeeRoundAction(int playerId) : RoundAction(playerId)
{
    public class FireAnEmployeePayload
    {
        public int EmployeeId { get; init; }
    }

    public FireAnEmployeePayload Payload { get; private set; } = null!;

    protected override void ApplyPayload(JsonObject payload)
    {
        Payload = JsonSerializer.Deserialize<FireAnEmployeePayload>(payload)!;
    }
}

public class PassMyTurnRoundAction(int playerId) : RoundAction(playerId)
{
    protected override void ApplyPayload(JsonObject payload)
    {
    }
}
