﻿namespace WebApi.Services;

public interface ICacheService
{
    T GetData<T>(string key);
    bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
    void RemoveData(string key);
}
