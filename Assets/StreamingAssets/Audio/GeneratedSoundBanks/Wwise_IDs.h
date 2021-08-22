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
        static const AkUniqueID PLAY_MARIE_ATTACK = 822824111U;
        static const AkUniqueID PLAY_MOUNTAIN_TRAIL_THEME = 3399001768U;
        static const AkUniqueID PLAY_NILES_ATTACK = 1713481986U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace COMBATSTATE
        {
            static const AkUniqueID GROUP = 1071758680U;

            namespace STATE
            {
                static const AkUniqueID IN_COMBAT = 2116791127U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID OUT_OF_COMBAT = 3162566204U;
            } // namespace STATE
        } // namespace COMBATSTATE

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
