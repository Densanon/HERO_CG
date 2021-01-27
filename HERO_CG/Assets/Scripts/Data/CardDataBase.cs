using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDataBase : MonoBehaviour
{
    [SerializeField] private Image[] HeroImages = new Image[20];
    [SerializeField] private Image[] AbilityImages = new Image[20];
    [SerializeField] private Image[] EnhanceImages = new Image[4];
    [SerializeField] private Image[] FeatImages = new Image[4];

    Card[] Heros = new Card[20];
    Card[] Abilities = new Card[20];
    Card[] Enhancements = new Card[4];
    Card[] Feats = new Card[4];

    private void Awake()
    {
        Heros[0] = new Card(Card.Type.Character, "AKIO", 20, 70, "<< Societal freedom is a mirage for true hope. >>", "(P) When Akio causes another hero to be fatigued, Akio is not fatigued. If Akio attacks and causes the hero he attacks to become fatigued, Akio is not fatigued and may continue attacking.", HeroImages[0]);
        Heros[1] = new Card(Card.Type.Character, "AYUMI", 40, 50, "<< The River will meet all your needs. >>", "(P) Whenever a hero is recruited, you may draw a card from your Enhancement Deck. If a player takes a Recruit Action and they choose to recruit 2 heroes, you may draw 2 cards.", HeroImages[1]);
        Heros[2] = new Card(Card.Type.Character, "BOULOS",90, 0, "<< The dystopic is our past, present, and future--yet there is still hope. >>", "For each card in your hand, Boulos gains +10 defense.", HeroImages[2]);
        Heros[3] = new Card(Card.Type.Character, "CHRISTOPH", 30, 60, "<< Vraiment? Encore? Ok, je pensais qu'ils retiendraient la le(c)on au bout d'un moment. >>", " (P) When an attack against Christoph is resolved, you may choose one of the attacking heroes to be defeated.", HeroImages[3]);
        Heros[4] = new Card(Card.Type.Character, "ENG", 10, 80, "<< CHINESE >>", "(A) After an Action, heal one hero.", HeroImages[4]);
        Heros[5] = new Card(Card.Type.Character, "GAMBITO", 50, 40, "<< A veces, no hay negociacion. >>", "(P) When any hero is fatigued, all players must discard a random card from their hand.", HeroImages[5]);
        Heros[6] = new Card(Card.Type.Character, "GRIT", 0 , 90, "<< Broken clay must be revived before it can be used to create. >>", "(P) Grit may gain +20 attack for every fatigued hero. For every single hero on the field in a fatigued position before the start of the Attack, Grit gains +20 attack.", HeroImages[6]);
        Heros[7] = new Card(Card.Type.Character, "HINDRA",70, 20, "<< You're missing the point here. There's so much more to Ghostwalking than just jumping on air. >>", "(A) Choose one opponent to prevent from playing Ability Cards to the field on their next turn.", HeroImages[7]);
        Heros[8] = new Card(Card.Type.Character, "IGNACIA", 20, 70, "<< You are capable of greatness. Capable of shifting the course of history. Capable of bringing loife to the dead. >>", "(A) Ignacia may attack once per turn while fatigued (any strengthened allies may join the attack and become fatigued).", HeroImages[8]);
        Heros[9] = new Card(Card.Type.Character, "ISAAC", 40, 50, "(Hebrew)", "(P) When another hero is defeated, you may draw a card from your Discard Pile that was there before the hero was defeated.", HeroImages[9]);
        Heros[10] = new Card(Card.Type.Character, "IZUMI", 10, 80, "<< Don't Worry. I'm not going anywhere. >>", "(P) Allied heroes may gain +20 defense while Izumi is in play. Fatigued heroes gain this after their defense is halved.", HeroImages[10]);
        Heros[11] = new Card(Card.Type.Character, "KAY", 50, 40, "<< Are you ready yet? >>", "(A) Play a card to the field from your hand.", HeroImages[11]);
        Heros[12] = new Card(Card.Type.Character, "KYAUTA", 90, 0, "<< Yare daye basai isa ba. >>", "(A) After fully completing the Action you chose for your turn, fatigue one of your stengthened heroes. Then, recruit one hero directly to your play area from the top of the Reserves pile.", HeroImages[12]);
        Heros[13] = new Card(Card.Type.Character, "MACE", 0, 90, "<< A pillar is only as strong as it's foundation. >>", "(P) You may double the Total Attack for one attack that occurs on your turn.", HeroImages[13]);
        Heros[14] = new Card(Card.Type.Character, "MICHAEL", 80, 10, "<< A place of rest is a geyser of drive. >>", "(A) Draw a card from your Enhancement Deck to your hand.", HeroImages[14]);
        Heros[15] = new Card(Card.Type.Character, "ORIGIN", 80, 10, "<< Let's just say, I've been a mother for a very long time. >>", "(P) During each opponent’s turn, you may block one attack against Origin.", HeroImages[15]);
        Heros[16] = new Card(Card.Type.Character, "ROHAN", 70,20, "<< If you step on board the Redux, you can be sure that there will be storms ahead. >>", "(A) For each fatigued hero, recruit one hero from Hero HQ or Reserves to your hand.", HeroImages[16]);
        Heros[17] = new Card(Card.Type.Character, "YASMINE", 60, 30, "<< (Egyptian?) >>", "(P) When an attack against Yasmine is resolved, you may play a card to the field.", HeroImages[17]);
        Heros[18] = new Card(Card.Type.Character, "ZHAO", 70, 20, "<< Time to jumpstart this fallen world. >>", "(P) As long as Zhao is in play, all Allied heroes may gain +10 attack. (P) When Zhao is defeated/removed, you can play a card from your hand to the field.", HeroImages[18]);
        Heros[19] = new Card(Card.Type.Character, "ZOE", 50, 40, "<< (Hebrew) >>", "(A) Before performing an Action, heal any hero(es) of your choice.", HeroImages[19]);

        Abilities[0] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[0]);
        Abilities[1] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[1]);
        Abilities[2] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[2]);
        Abilities[3] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[3]);
        Abilities[4] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[4]);
        Abilities[5] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[5]);
        Abilities[6] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[6]);
        Abilities[7] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[7]);
        Abilities[8] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[8]);
        Abilities[9] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[9]);
        Abilities[10] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[10]);
        Abilities[11] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[11]);
        Abilities[12] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[12]);
        Abilities[13] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[13]);
        Abilities[14] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[14]);
        Abilities[15] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[15]);
        Abilities[16] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[16]);
        Abilities[17] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[17]);
        Abilities[18] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[18]);
        Abilities[19] = new Card(Card.Type.Ability, "Name", "Flavor", "Ability", AbilityImages[19]);

        Enhancements[0] = new Card(Card.Type.Enhancement, "Attack 20", 20, 0, EnhanceImages[0]);
        Enhancements[1] = new Card(Card.Type.Enhancement, "Attack 30", 30, 0, EnhanceImages[1]);
        Enhancements[2] = new Card(Card.Type.Enhancement, "Defense 30", 0, 30, EnhanceImages[2]);
        Enhancements[3] = new Card(Card.Type.Enhancement, "Defense 20", 0, 20, EnhanceImages[3]);


        Feats[0] = new Card(Card.Type.Feat, "Absorb", "(H) Discard the Enhancements, if any, from any one hero. Then, replace with those from another hero.", FeatImages[0]);
        Feats[1] = new Card(Card.Type.Feat, "Drain", "(H) Discard all of one opponenet's Enhancement Cards from the field.", FeatImages[1]);
        Feats[2] = new Card(Card.Type.Feat, "Pay the Cost", "(H) Fatigue one hero in your play area, to remove one hero from the field.", FeatImages[2]);
        Feats[3] = new Card(Card.Type.Feat, "Under Siege", "(H) Target opponent reveals their hand, then discards all non-hero cards.", FeatImages[3]);
    }
}
