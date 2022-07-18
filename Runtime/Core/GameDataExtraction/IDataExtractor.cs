using System;
using Match3.Core.TurnSteps;

namespace Match3.Core.GameDataExtraction
{
    public interface IDataExtractor
    {
        bool CanExtractDataFrom(TurnStep turnStep);
        IExtractedData ExtractData(TurnStep turnStep);
    }
    
    public abstract class DataExtractorBase<TT, TD> : IDataExtractor where TT : TurnStep where TD : IExtractedData
    {
        public bool CanExtractDataFrom(TurnStep turnStep)
        {
            return turnStep is TT;
        }

        public IExtractedData ExtractData(TurnStep turnStep)
        {
            if (turnStep is TT tt)
            {
                return ExtractData(tt);
            }
            
            throw new ArgumentException(
                $"Wrong turn step type {turnStep.GetType().Name}. Expected type is {typeof(TT).Name}");
        }

        public abstract TD ExtractData(TT turnStep);
    }
}