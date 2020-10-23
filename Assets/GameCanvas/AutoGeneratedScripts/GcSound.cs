/*------------------------------------------------------------*/
// <summary>GameCanvas for Unity</summary>
// <author>Seibe TAKAHASHI</author>
// <remarks>
// (c) 2015-2020 Smart Device Programming.
// This software is released under the MIT License.
// http://opensource.org/licenses/mit-license.php
// </remarks>
/*------------------------------------------------------------*/
namespace GameCanvas
{
    public readonly partial struct GcSound : System.IEquatable<GcSound>
    {
        internal const int __Length__ = 7;
        public static readonly GcSound Bgm_game = new GcSound("GcSoundBgm_game");
        public static readonly GcSound Se_clear = new GcSound("GcSoundSe_clear");
        public static readonly GcSound Se_explosion = new GcSound("GcSoundSe_explosion");
        public static readonly GcSound Se_flagged = new GcSound("GcSoundSe_flagged");
        public static readonly GcSound Se_gameover = new GcSound("GcSoundSe_gameover");
        public static readonly GcSound Se_open = new GcSound("GcSoundSe_open");
        public static readonly GcSound Se_title = new GcSound("GcSoundSe_title");
    }
}
