/*******************************************************************************
 * Copyright (C) 2018-2022 MarbleBag
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free
 * Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>
 *
 * SPDX-License-Identifier: AGPL-3.0-or-later
 *******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace NexusVault.Format.Bin
{

    public sealed class Locale : IEquatable<Locale>
    {
        private string _tagName;
        private string _shortName;
        private string _longName;

        public int Type { get; set; }
        public string TagName
        {
            get => _tagName;
            set => _tagName = value ?? throw new ArgumentNullException(nameof(TagName));
        }

        public string ShortName
        {
            get => _shortName;
            set => _shortName = value ?? throw new ArgumentNullException(nameof(ShortName));
        }

        public string LongName
        {
            get => _longName;
            set => _longName = value ?? throw new ArgumentNullException(nameof(LongName));
        }

        public Locale(int type, string tagName, string shortName, string longName)
        {
            Type = type;
            TagName = tagName;
            ShortName = shortName;
            LongName = longName;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Locale);
        }

        public bool Equals(Locale other)
        {
            return other != null &&
                   Type == other.Type &&
                   TagName == other.TagName &&
                   ShortName == other.ShortName &&
                   LongName == other.LongName;
        }

        public override int GetHashCode()
        {
            var hashCode = -546380389;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TagName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ShortName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LongName);
            return hashCode;
        }
    }

    public sealed class LanguageDictionary : IEquatable<LanguageDictionary>
    {
        public Locale Locale { get; }

        public Dictionary<uint, string> Entries { get; }

        public LanguageDictionary(Locale locale, Dictionary<uint, string> entries)
        {
            Locale = locale ?? throw new ArgumentNullException(nameof(locale));
            Entries = entries ?? throw new ArgumentNullException(nameof(entries));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as LanguageDictionary);
        }

        public bool Equals(LanguageDictionary other)
        {
            return other != null &&
                   EqualityComparer<Locale>.Default.Equals(Locale, other.Locale) &&
                   Entries.Count == Entries.Count && !Entries.Except(Entries).Any();
        }

        public override int GetHashCode()
        {
            var hashCode = 1586501090;
            hashCode = hashCode * -1521134295 + EqualityComparer<Locale>.Default.GetHashCode(Locale);
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<uint, string>>.Default.GetHashCode(Entries);
            return hashCode;
        }
    }
}
