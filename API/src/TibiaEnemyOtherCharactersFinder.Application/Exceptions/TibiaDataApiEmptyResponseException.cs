﻿namespace TibiaEnemyOtherCharactersFinder.Application.Exceptions;

public class TibiaDataApiEmptyResponseException : TibiaEnemyOtherCharacterFinderException
{
    public TibiaDataApiEmptyResponseException() : base("Tibia Data Api response with empty object and status code 200.")
    {
    }
}