﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Xml;

using ICSharpCode.TextEditor.Util;

namespace ICSharpCode.TextEditor.Document
{
    public class HighlightRuleSet
    {
        #region Fields
        internal HighlightingStrategy Highlighter;
        #endregion

        #region Properties
        public ArrayList Spans { get; private set; } = new ArrayList();

        public LookupTable KeyWords { get; } = new LookupTable(false);

        public LookupTable PrevMarkers { get; } = new LookupTable(false);

        public LookupTable NextMarkers { get; } = new LookupTable(false);

        public bool[] Delimiters { get; } = new bool[256]; //TODOsyntax fix this later

        public char EscapeCharacter { get; }

        public bool IgnoreCase { get; } = false;

        public string Name { get; set; } = null;

        public string Reference { get; } = null;
        #endregion

        #region Lifecycle
        public HighlightRuleSet()
        {
        }

        public HighlightRuleSet(XmlElement el)
        {
            XmlNodeList nodes;

            if (el.Attributes["name"] != null)
            {
                Name = el.Attributes["name"].InnerText;
            }

            if (el.HasAttribute("escapecharacter"))
            {
                EscapeCharacter = el.GetAttribute("escapecharacter")[0];
            }

            if (el.Attributes["reference"] != null)
            {
                Reference = el.Attributes["reference"].InnerText;
            }

            if (el.Attributes["ignorecase"] != null)
            {
                IgnoreCase  = Boolean.Parse(el.Attributes["ignorecase"].InnerText);
            }

            for (int i  = 0; i < Delimiters.Length; ++i)
            {
                Delimiters[i] = false;
            }

            if (el["Delimiters"] != null)
            {
                string delimiterString = el["Delimiters"].InnerText;
                foreach (char ch in delimiterString)
                {
                    Delimiters[(int)ch] = true;
                }
            }

//			Spans       = new LookupTable(!IgnoreCase);

            KeyWords    = new LookupTable(!IgnoreCase);
            PrevMarkers = new LookupTable(!IgnoreCase);
            NextMarkers = new LookupTable(!IgnoreCase);

            nodes = el.GetElementsByTagName("KeyWords");
            foreach (XmlElement el2 in nodes)
            {
                HighlightColor color = new HighlightColor(el2);

                XmlNodeList keys = el2.GetElementsByTagName("Key");
                foreach (XmlElement node in keys)
                {
                    KeyWords[node.Attributes["word"].InnerText] = color;
                }
            }

            nodes = el.GetElementsByTagName("Span");
            foreach (XmlElement el2 in nodes)
            {
                Spans.Add(new Span(el2));
                /*
                Span span = new Span(el2);
                Spans[span.Begin] = span;*/
            }

            nodes = el.GetElementsByTagName("MarkPrevious");
            foreach (XmlElement el2 in nodes)
            {
                AdjacentMarker prev = new AdjacentMarker(el2);
                PrevMarkers[prev.What] = prev;
            }

            nodes = el.GetElementsByTagName("MarkFollowing");
            foreach (XmlElement el2 in nodes)
            {
                AdjacentMarker next = new AdjacentMarker(el2);
                NextMarkers[next.What] = next;
            }
        }
        #endregion

        /// <summary>
        /// Merges spans etc. from the other rule set into this rule set.
        /// </summary>
        public void MergeFrom(HighlightRuleSet ruleSet)
        {
            for (int i = 0; i < Delimiters.Length; i++)
            {
                Delimiters[i] |= ruleSet.Delimiters[i];
            }
            // insert merged spans in front of old spans
            ArrayList oldSpans = Spans;
            Spans = (ArrayList)ruleSet.Spans.Clone();
            Spans.AddRange(oldSpans);
            //keyWords.MergeFrom(ruleSet.keyWords);
            //prevMarkers.MergeFrom(ruleSet.prevMarkers);
            //nextMarkers.MergeFrom(ruleSet.nextMarkers);
        }
    }
}
