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

[JsonDerivedType(typeof(RoundActionType), typeDiscriminator: "RoundAction")]
[JsonDerivedType(typeof(SendEmployeeForTrainingRoundAction), typeDiscriminator: "SendEmployeeForTraining")]
[JsonDerivedType(typeof(ParticipateInCallForTendersRoundAction), typeDiscriminator: "ParticipateInCallForTenders")]
[JsonDerivedType(typeof(RecruitAConsultantRoundAction), typeDiscriminator: "RecruitAConsultant")]
[JsonDerivedType(typeof(FireAnEmployeeRoundAction), typeDiscriminator: "FireAnEmployee")]
[JsonDerivedType(typeof(PassMyTurnRoundAction), typeDiscriminator: "PassMyTurn")]
public class RoundAction(RoundActionType actionType)
{
    public RoundActionType ActionType { get; init; } = actionType;
}

public class SendEmployeeForTrainingRoundAction() : RoundAction(RoundActionType.SendEmployeeForTraining);
public class ParticipateInCallForTendersRoundAction() : RoundAction(RoundActionType.ParticipateInCallForTenders);
public class RecruitAConsultantRoundAction() : RoundAction(RoundActionType.RecruitAConsultant);
public class FireAnEmployeeRoundAction() : RoundAction(RoundActionType.FireAnEmployee);
public class PassMyTurnRoundAction() : RoundAction(RoundActionType.PassMyTurn);
