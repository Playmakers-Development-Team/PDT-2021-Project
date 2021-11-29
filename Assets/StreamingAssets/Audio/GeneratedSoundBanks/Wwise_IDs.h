/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID FRANCICO_SUCKZ = 2896490611U;
        static const AkUniqueID FRANCISCO_SUCKS = 3610251637U;
        static const AkUniqueID IN_COMBAT = 2116791127U;
        static const AkUniqueID OUT_OF_COMBAT = 3162566204U;
        static const AkUniqueID PLAY_ALLY_SPAWN = 1515718740U;
        static const AkUniqueID PLAY_BATTLEFIELD_CURSOR = 1114270429U;
        static const AkUniqueID PLAY_BRUSH_STROKE = 4184699073U;
        static const AkUniqueID PLAY_CAMPFIRE_AMBIENCE = 2984727828U;
        static const AkUniqueID PLAY_CREDITS_THEME = 4043016790U;
        static const AkUniqueID PLAY_ESTELLE_ABILITY = 701627935U;
        static const AkUniqueID PLAY_GRASS_FOOTSTEPS = 546793308U;
        static const AkUniqueID PLAY_HELENA_ABILITY = 2350892418U;
        static const AkUniqueID PLAY_MARIE_ATTACK = 822824111U;
        static const AkUniqueID PLAY_MENU_DESELECT = 3771192879U;
        static const AkUniqueID PLAY_MENU_HOVER = 3835684012U;
        static const AkUniqueID PLAY_MENU_SELECT = 2167056030U;
        static const AkUniqueID PLAY_MOUNTAIN_TRAIL_THEME = 3399001768U;
        static const AkUniqueID PLAY_NEGATIVE_HIT = 2406652529U;
        static const AkUniqueID PLAY_NILES_ABILITY = 290037920U;
        static const AkUniqueID PLAY_NILES_ATTACK = 1713481986U;
        static const AkUniqueID PLAY_OPENING_THEME = 491678864U;
        static const AkUniqueID PLAY_PAGE_TURN = 786024383U;
        static const AkUniqueID PLAY_POSITIVE_HIT = 2175118573U;
        static const AkUniqueID PLAY_RUINED_CITY_THEME = 1017112381U;
        static const AkUniqueID PLAY_TERRAIN_SPAWN = 643292685U;
        static const AkUniqueID PLAY_TURN_MANIPULATION_TOGGLE = 495033984U;
        static const AkUniqueID PLAY_UI_SLIDING = 3062144419U;
        static const AkUniqueID PLAY_UNIT_DESPAWN = 3788644491U;
        static const AkUniqueID PLAY_UNIT_SPAWN = 4133036576U;
        static const AkUniqueID PLAY_UNIT_WALKING = 143520528U;
        static const AkUniqueID PLAY_WATERCOLOUR = 990444947U;
        static const AkUniqueID STOP_CREDITS_THEME = 3722690752U;
        static const AkUniqueID STOP_MOUNTAIN_TRAIL_THEME = 1697649638U;
        static const AkUniqueID STOP_OPENING_THEME = 3300588166U;
        static const AkUniqueID STOP_RUINED_CITY_THEME = 1288752611U;
        static const AkUniqueID STOP_UNIT_WALKING = 1987360222U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace COMBATSTATE
        {
            static const AkUniqueID GROUP = 1071758680U;

            namespace STATE
            {
                static const AkUniqueID IN_COMBAT = 2116791127U;
                static const AkUniqueID INPAUSEMENU = 2876317275U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID OUT_OF_COMBAT = 3162566204U;
            } // namespace STATE
        } // namespace COMBATSTATE

        namespace LEVELSTATE
        {
            static const AkUniqueID GROUP = 3473087568U;

            namespace STATE
            {
                static const AkUniqueID CITYSCAPE = 2337594226U;
                static const AkUniqueID CREDITS = 2201105581U;
                static const AkUniqueID MAINMENU = 3604647259U;
                static const AkUniqueID MOUNTAINTRAIL = 730363388U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace LEVELSTATE

        namespace PLAYERLIFE
        {
            static const AkUniqueID GROUP = 444815956U;

            namespace STATE
            {
                static const AkUniqueID ALIVE = 655265632U;
                static const AkUniqueID DEAD = 2044049779U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace PLAYERLIFE

        namespace WALKINGSTATE
        {
            static const AkUniqueID GROUP = 979933241U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID NOT_WALKING = 1093676608U;
                static const AkUniqueID WALKING = 340271938U;
            } // namespace STATE
        } // namespace WALKINGSTATE

    } // namespace STATES

    namespace SWITCHES
    {
        namespace GAMEPLAY_SWITCH
        {
            static const AkUniqueID GROUP = 2702523344U;

            namespace SWITCH
            {
                static const AkUniqueID COMBAT = 2764240573U;
                static const AkUniqueID EXPLORE = 579523862U;
            } // namespace SWITCH
        } // namespace GAMEPLAY_SWITCH

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID MASTERVOLUME = 2918011349U;
        static const AkUniqueID MUSICVOLUME = 2346531308U;
        static const AkUniqueID SFXVOLUME = 988953028U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MAIN_MENU = 2005704188U;
        static const AkUniqueID PLACEHOLDER = 1548734028U;
        static const AkUniqueID SAMPLE = 601528575U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID ATMOS_BUS = 389067731U;
        static const AkUniqueID DX_BUS = 1748021847U;
        static const AkUniqueID FOLEY_BUS = 4213046568U;
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MUSIC_BUS = 3127962312U;
        static const AkUniqueID SFX_BUS = 1502772432U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
