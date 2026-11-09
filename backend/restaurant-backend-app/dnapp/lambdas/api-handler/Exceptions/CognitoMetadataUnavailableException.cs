using System;

namespace APIHandler.Exceptions;

public class CognitoMetadataUnavailableException(Exception innerException)
    : Exception("Cognito signing metadata is temporarily unavailable.", innerException);
