#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using SourceAFIS;

namespace ZkTecoFingerPrint;

public static class Extensions
{
    private static readonly SHA1 Sha1 = SHA1.Create();
    private static readonly ConcurrentDictionary<string, FingerprintMatcher> Matchers = new();

    private static FingerprintMatcher GetMatcher(this ZkFingerPrintResult fingerPrintResult)
    {
        return Matchers.GetOrAdd(fingerPrintResult.TemplateHash,
                                  _ => new FingerprintMatcher(fingerPrintResult.Template));
    }

    public static double Match(this ZkFingerPrintResult fingerPrintResult, FingerprintTemplate template)
    {
        return fingerPrintResult
              .GetMatcher()
              .Match(template);
    }

    public static TSubject? Identify<TSubject>(this ZkFingerPrintResult fingerPrintResult,
                                               IEnumerable<TSubject> candidates,
                                               Func<TSubject, FingerprintTemplate> templateSelector,
                                               double threshold = 40)
    {
        var matcher = fingerPrintResult.GetMatcher();

        return candidates
              .Select(x =>
                      {
                          var template = templateSelector(x);
                          var similarity = matcher.Match(template);
                          return (Subject: x, Similiraty: similarity);
                      })
              .Where(x => x.Similiraty >= threshold)
              .OrderByDescending(x => x.Similiraty)
              .FirstOrDefault()
              .Subject;
    }

    public static string Hash(byte[] value)
    {
        var bytes = Sha1.ComputeHash(value);
        return BitConverter.ToString(bytes);
    }
}