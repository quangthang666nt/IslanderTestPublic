using System.Collections.Generic;
using UnityEngine;

public class SCSCommonTranslatedTexts : MonoBehaviour
{
	public enum CommonDialogMessage
	{
		AUTOSAVE_ENABLED = 0,
		PRESS_ANY_BUTTON = 1,
		LOADING = 2,
		BUTTON_TRANSLATED = 3
	}

	private enum LanguageID
	{
		English = 0,
		Spanish = 1,
		French = 2,
		German = 3,
		Italian = 4,
		Portuguese = 5,
		Dutch = 6,
		Turkish = 7,
		Polish = 8,
		Finnish = 9,
		Russian = 10,
		Japanese = 11,
		Chinese_S = 12,
		Chinese_T = 13,
		Korean = 14
	}

	private LanguageID currentLang;

	private Dictionary<CommonDialogMessage, Dictionary<LanguageID, string>> messages;

	public static SCSCommonTranslatedTexts Instance { get; private set; }

	public void Start()
	{
		Instance = this;
		messages = new Dictionary<CommonDialogMessage, Dictionary<LanguageID, string>>();
		messages.Add(CommonDialogMessage.AUTOSAVE_ENABLED, AutoSaveMessage());
		messages.Add(CommonDialogMessage.PRESS_ANY_BUTTON, PressAnyButton());
		messages.Add(CommonDialogMessage.LOADING, Loading());
		messages.Add(CommonDialogMessage.BUTTON_TRANSLATED, Button());
		currentLang = CheckSystemLanguage();
	}

	private LanguageID CheckSystemLanguage()
	{
		return Application.systemLanguage switch
		{
			SystemLanguage.Spanish => LanguageID.Spanish, 
			SystemLanguage.French => LanguageID.French, 
			SystemLanguage.German => LanguageID.German, 
			SystemLanguage.Italian => LanguageID.Italian, 
			SystemLanguage.Portuguese => LanguageID.Portuguese, 
			SystemLanguage.Dutch => LanguageID.Dutch, 
			SystemLanguage.Turkish => LanguageID.Turkish, 
			SystemLanguage.Russian => LanguageID.Russian, 
			SystemLanguage.Japanese => LanguageID.Japanese, 
			SystemLanguage.ChineseSimplified => LanguageID.Chinese_S, 
			SystemLanguage.ChineseTraditional => LanguageID.Chinese_T, 
			SystemLanguage.Korean => LanguageID.Korean, 
			_ => LanguageID.English, 
		};
	}

	public string GetTranslatedPhrase(CommonDialogMessage nextPhrase)
	{
		if (messages[nextPhrase].ContainsKey(currentLang))
		{
			return messages[nextPhrase][currentLang];
		}
		return messages[nextPhrase][LanguageID.English];
	}

	private Dictionary<LanguageID, string> AutoSaveMessage()
	{
		return new Dictionary<LanguageID, string>
		{
			{
				LanguageID.English,
				"This game has Auto Save enabled. Please, do not turn off your console when you see this icon"
			},
			{
				LanguageID.Spanish,
				"Este juego tiene habilitada la función de guardado automático. Por favor, no apagues la consola cuando veas este icono"
			},
			{
				LanguageID.French,
				"La sauvegarde automatique est activée sur ce jeu. Veuillez ne pas arrêter votre console lorsque vous verrez cette icône"
			},
			{
				LanguageID.German,
				"Dieses Spiel speichert automatisch. Bitte schalte nicht die Konsole aus, wenn du dieses Icon siehst"
			},
			{
				LanguageID.Italian,
				"Questo gioco ha la funzione di salvataggio automatico abilitata. Non spegnere la console quando vedi quest'icona"
			},
			{
				LanguageID.Portuguese,
				"Este jogo tem o Guardar Automático ativo. Por favor, não desligue a sua consola quando vir este ícone"
			},
			{
				LanguageID.Dutch,
				"Automatisch Opslaan is ingeschakeld voor dit spel. Schakel je console niet uit als je dit icoontje ziet"
			},
			{
				LanguageID.Turkish,
				"Bu oyunda Otomatik Kaydetme etkindir. Lütfen bu simgeyi gördüğünüzde konsolonuzu kapatmayın"
			},
			{
				LanguageID.Russian,
				"В этой игре включено автосохранение. Не выключайте свою приставку, когда увидите этот значок"
			},
			{
				LanguageID.Japanese,
				"このゲームでは自動セーブが機能します。このアイコンが表示されている最中は、ゲーム機をオフにしないでください"
			}
		};
	}

	private Dictionary<LanguageID, string> PressAnyButton()
	{
		return new Dictionary<LanguageID, string>
		{
			{
				LanguageID.English,
				"Press any button to continue"
			},
			{
				LanguageID.Spanish,
				"Pulsa cualquier botón para continuar"
			},
			{
				LanguageID.French,
				"Appuyez sur n'importe quel bouton pour continuer"
			},
			{
				LanguageID.German,
				"Drücke eine Taste, um fortzufahren"
			},
			{
				LanguageID.Italian,
				"Premi un tasto qualsiasi per continuare"
			},
			{
				LanguageID.Portuguese,
				"Pressione qualquer botão para continuar"
			},
			{
				LanguageID.Dutch,
				"Druk op een willekeurige knop om door te gaan"
			},
			{
				LanguageID.Turkish,
				"Devam etmek için herhangi bir butona tıklayın"
			},
			{
				LanguageID.Russian,
				"Нажмите любую кнопку,  чтобы продолжить"
			},
			{
				LanguageID.Japanese,
				"どれでもボタンを押して続行"
			}
		};
	}

	private Dictionary<LanguageID, string> Loading()
	{
		return new Dictionary<LanguageID, string>
		{
			{
				LanguageID.English,
				"Loading…"
			},
			{
				LanguageID.Spanish,
				"Cargando…"
			},
			{
				LanguageID.French,
				"Chargement..."
			},
			{
				LanguageID.German,
				"Spiel lädt..."
			},
			{
				LanguageID.Italian,
				"Caricamento…"
			},
			{
				LanguageID.Portuguese,
				"A carregar…"
			},
			{
				LanguageID.Dutch,
				"Aan het laden…"
			},
			{
				LanguageID.Turkish,
				"Yükleniyor…"
			},
			{
				LanguageID.Russian,
				"Загрузка…"
			},
			{
				LanguageID.Japanese,
				"ロード中…"
			},
			{
				LanguageID.Chinese_S,
				"正在加载……"
			},
			{
				LanguageID.Chinese_T,
				"正在載入……"
			},
			{
				LanguageID.Korean,
				"불러오는 중..."
			}
		};
	}

	private Dictionary<LanguageID, string> Button()
	{
		return new Dictionary<LanguageID, string>
		{
			{
				LanguageID.English,
				"Button"
			},
			{
				LanguageID.Spanish,
				"Botón"
			},
			{
				LanguageID.French,
				"Touche"
			},
			{
				LanguageID.German,
				"Taste"
			},
			{
				LanguageID.Italian,
				"Tasto"
			},
			{
				LanguageID.Portuguese,
				"Botão"
			},
			{
				LanguageID.Dutch,
				"Toets"
			},
			{
				LanguageID.Turkish,
				"düğmesi"
			},
			{
				LanguageID.Russian,
				"кнопка"
			},
			{
				LanguageID.Japanese,
				"ボタン"
			}
		};
	}

	private Dictionary<LanguageID, string> NoPs4OnePlayer()
	{
		return new Dictionary<LanguageID, string>
		{
			{
				LanguageID.English,
				"No Xbox One player is signed in."
			},
			{
				LanguageID.Spanish,
				"Ningún jugador de Xbox One conectado."
			},
			{
				LanguageID.French,
				"Aucun joueur de Xbox One n'a été connecté."
			},
			{
				LanguageID.German,
				"Kein Xbox One spieler angemeldet."
			},
			{
				LanguageID.Italian,
				"Non c'è alcun giocatore Xbox One connesso."
			},
			{
				LanguageID.Portuguese,
				"Nenhum jogador do Xbox One está logado."
			},
			{
				LanguageID.Turkish,
				"Hiçbir XBox One oyuncusu giriş yapmamış."
			},
			{
				LanguageID.Russian,
				"Не авторизован ни один игрок Xbox One."
			},
			{
				LanguageID.Chinese_S,
				"无 Xbox One 玩家登录。"
			},
			{
				LanguageID.Chinese_T,
				"無 Xbox One 玩家登入。"
			},
			{
				LanguageID.Korean,
				"로그인한 XBbox One 플레이어가 없습니다."
			}
		};
	}

	private Dictionary<LanguageID, string> NoXboxOnePlayer()
	{
		return new Dictionary<LanguageID, string>
		{
			{
				LanguageID.English,
				"No Xbox One profile is signed in."
			},
			{
				LanguageID.Spanish,
				"Ningún perfil de Xbox One conectado."
			},
			{
				LanguageID.French,
				"Aucun profil de Xbox One n'a été connecté."
			},
			{
				LanguageID.German,
				"Kein Xbox One profil angemeldet."
			},
			{
				LanguageID.Italian,
				"Non c'è alcun profilo Xbox One connesso."
			},
			{
				LanguageID.Portuguese,
				"Nenhum perfil do Xbox One está logado."
			},
			{
				LanguageID.Turkish,
				"Hiçbir XBox One profil giriş yapmamış."
			},
			{
				LanguageID.Russian,
				"Не авторизован ни один профиль Xbox One."
			},
			{
				LanguageID.Chinese_S,
				"无 Xbox One 档案登录。"
			},
			{
				LanguageID.Chinese_T,
				"無 Xbox One 設定檔登入。"
			},
			{
				LanguageID.Korean,
				"로그인한 XBbox One 프로필 없습니다."
			}
		};
	}

	private Dictionary<LanguageID, string> WillLoseProgress()
	{
		return new Dictionary<LanguageID, string>
		{
			{
				LanguageID.English,
				"Your progress will be lost!"
			},
			{
				LanguageID.Spanish,
				"Tu progreso se perderá."
			},
			{
				LanguageID.French,
				"Vos progrès seront perdus!"
			},
			{
				LanguageID.German,
				"Dein Fortschritt wird gelöscht!"
			},
			{
				LanguageID.Italian,
				"I tuoi progressi andranno persi!"
			},
			{
				LanguageID.Portuguese,
				"Você vai perder o seu progresso!"
			},
			{
				LanguageID.Turkish,
				"İlerlemen kaybolacak!"
			},
			{
				LanguageID.Russian,
				"Ваш прогресс будет потерян!"
			}
		};
	}

	private Dictionary<LanguageID, string> SignBackInOrBackToMenu()
	{
		return new Dictionary<LanguageID, string>
		{
			{
				LanguageID.English,
				"Sign back in to resume the game or log in with another user and return to main menu."
			},
			{
				LanguageID.Spanish,
				"Conéctate de nuevo para reanudar el juego o inicia sesión con otro usuario y volver al menú principal"
			},
			{
				LanguageID.French,
				"Reconnectez-vous pour reprendre le jeu ou connectez-vous avec un autre utilisateur et revenez au menu principal."
			},
			{
				LanguageID.German,
				"Melde dich wieder an, um das Spiel fortzusetzen oder logge dich mit einem anderen Benutzer ein und kehre ins Hauptmenü zurück."
			},
			{
				LanguageID.Italian,
				"Accedi nuovamente per riprendere la partita o accedi con un altro utente e torna al menu principale."
			},
			{
				LanguageID.Portuguese,
				"Entre novamente para retomar o jogo ou faça login com outro usuário e volte ao menu principal."
			},
			{
				LanguageID.Turkish,
				"Oyuna devam etmek için tekrar giriş yap veya başka bir kullanıcıyla oturum aç ve ana menüye geri dön."
			},
			{
				LanguageID.Russian,
				"Авторизуйтесь, чтобы возобновить игру, или введите данные для входа другого пользователя и вернитесь в главное меню."
			},
			{
				LanguageID.Chinese_S,
				"请重新登录来继续游戏，或以另一个用户名登录并返回至主菜单。"
			},
			{
				LanguageID.Chinese_T,
				"請重新登入來繼續遊戲，或以另一個用戶名登錄並返回至主選單。"
			},
			{
				LanguageID.Korean,
				"로그인하여 게임을 재개하거나 다른 사용자와 로그인 한 후 메인 메뉴로 돌아가세요."
			}
		};
	}

	private Dictionary<LanguageID, string> NoPS4Player()
	{
		return new Dictionary<LanguageID, string>
		{
			{
				LanguageID.English,
				"Please, connect a controller to continue."
			},
			{
				LanguageID.Spanish,
				"Por favor, conecta un mando para continuar."
			},
			{
				LanguageID.French,
				"Veuillez connecter une manette pour continuer."
			},
			{
				LanguageID.German,
				"Bitte schließen Sie einen Controller an, um fortzufahren."
			},
			{
				LanguageID.Turkish,
				"Devam etmek için lütfen bir kontrol cihazı bağlayın."
			},
			{
				LanguageID.Russian,
				"Чтобы продолжить, подключите контроллер."
			},
			{
				LanguageID.Chinese_S,
				"请连接控制器以继续。"
			},
			{
				LanguageID.Chinese_T,
				"請連接控制器以繼續。"
			},
			{
				LanguageID.Japanese,
				"続行するにはコントローラーを接続してください。"
			}
		};
	}
}
