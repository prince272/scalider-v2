using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Scalider;

namespace BCrypt.Net
{
    
    [UsedImplicitly]
    public sealed class HashInformation
    {

        private static readonly TimeSpan RegexTimeout =
            TimeSpan.FromMilliseconds(50);

        private static readonly Regex HashInformationRegex =
            new Regex(
                @"^\$(?<revision>2[a-z]{1}?)\$(?<workFactor>\d\d?)\$(?<salt>[A-Za-z0-9\./]{53})$",
                RegexOptions.Singleline, RegexTimeout);

        private HashInformation(SaltRevision revision, int workFactor, string salt, string rawHash)
        {
            Revision = revision;
            WorkFactor = workFactor;
            Salt = salt;
            RawHash = rawHash;
        }
        
        /// <summary>
        /// Gets a value indicating the salt revision of the hash.
        /// </summary>
        [UsedImplicitly]
        public SaltRevision Revision { get; }
        
        /// <summary>
        /// Gets a value indicating the work factor of the hash.
        /// </summary>
        [UsedImplicitly]
        public int WorkFactor { get; }
        
        /// <summary>
        /// Gets a value indicating the salt of the hash.
        /// </summary>
        [UsedImplicitly]
        public string Salt { get; }
        
        /// <summary>
        /// Gets a value indicating the hash.
        /// </summary>
        [UsedImplicitly]
        public string RawHash { get; }

        [UsedImplicitly]
        public static HashInformation Parse(string hash)
        {
            Check.NotNullOrEmpty(hash, nameof(hash));
            
            // Try to parse the hash information
            var matchResult = HashInformationRegex.Match(hash);
            if (!matchResult.Success ||
                !int.TryParse(matchResult.Groups["workFactor"].Value, out var workFactor) ||
                !TryGetSaltRevision(matchResult.Groups["revision"].Value, out var saltRevision))
            {
                // Could not parse the hash
                throw new HashInformationParseException("Invalid hash format");
            }
            
            // Done
            return new HashInformation(saltRevision, workFactor, matchResult.Groups["salt"].Value, hash);
        }

        [UsedImplicitly]
        public static bool TryParse(string hash, out HashInformation result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(hash))
                return false;

            // Try to parse the hash information
            try
            {
                result = Parse(hash);
                return true;
            }
            catch
            {
                // Ignore
            }
            
            // Could not parse the hash information
            return false;
        }

        /// <summary>
        /// Verifies that the hash of the given <paramref name="text"/> matches the <see cref="RawHash"/>.
        /// </summary>
        /// <param name="text">The text to verify.</param>
        /// <returns>
        /// <c>true</c> if the passwords match; otherwise <c>false</c>.
        /// </returns>
        public bool Verify(string text) => BCrypt.Verify(text, RawHash);

        /// <summary>
        /// Hash a password using the OpenBSD bcrypt scheme.
        /// </summary>
        /// <param name="input">The password to hash.</param>
        /// <returns>
        /// The hashed password.
        /// </returns>
        public string HashPassword(string input) =>
            BCrypt.HashPassword(
                input,
                string.Format("${0}${1:00}${2}", GetSaltRevisionString(Revision), WorkFactor, Salt)
            );

        private static bool TryGetSaltRevision(string value, out SaltRevision result)
        {
            result = SaltRevision.Revision2;
            if (string.IsNullOrWhiteSpace(value) || value[0] != '2' || value.Length > 2)
                return false;

            // Determine which version we got
            if (value.Length < 2)
                return true;
            
            switch (value[1])
            {
                case 'a':
                    result = SaltRevision.Revision2A;
                    break;
                case 'b':
                    result = SaltRevision.Revision2B;
                    break;
                case 'x':
                    result = SaltRevision.Revision2X;
                    break;
                case 'y':
                    result = SaltRevision.Revision2Y;
                    break;
                default:
                    return false;
            }
            
            // Got the salt revision successfully
            return true;
        }

        private static string GetSaltRevisionString(SaltRevision saltSaltRevision)
        {
            switch (saltSaltRevision)
            {
                case SaltRevision.Revision2:
                    return "2";
                case SaltRevision.Revision2A:
                    return "2a";
                case SaltRevision.Revision2B:
                    return "2b";
                case SaltRevision.Revision2X:
                    return "2x";
                case SaltRevision.Revision2Y:
                    return "2y";
                default:
                    throw new ArgumentOutOfRangeException(nameof(saltSaltRevision), saltSaltRevision, null);
            }
        }

    }
    
}