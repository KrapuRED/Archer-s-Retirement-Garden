using UnityEngine;
using UnityEngine.Pool;
using MoreMountains.Tools;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int starterCurrency;
    [SerializeField] private int currentCurrency;

    [Header("Audio Sound Effects")]
    [SerializeField] private AudioClip gainCurrencySound;
    [SerializeField] private AudioClip useCurrencySound;
    
    [Header("Currency Viusal Effect / Animation")] 
    [SerializeField] private CurrencyVisualizer currencyVisualizerPrefab;
    [SerializeField] private Transform spawnPosition;
    
    [Header("References")]
    [SerializeField] private CurrencyUI currencyUI;
    
    private IObjectPool<CurrencyVisualizer> _vfxPool;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        _vfxPool = new ObjectPool<CurrencyVisualizer>(
            createFunc: () => Instantiate(currencyVisualizerPrefab, spawnPosition),
            actionOnGet: (vfx) => vfx.gameObject.SetActive(true),
            actionOnRelease: (vfx) => vfx.gameObject.SetActive(false),
            actionOnDestroy: (vfx) => Destroy(vfx.gameObject),
            defaultCapacity: 10,
            maxSize: 50
        );
    }

    private void Start() => AddCurrency(starterCurrency);

    public void AddCurrency(int amountCurrency)
    {
        if (amountCurrency <= 0) return;
        
        CurrencyVisualizer vfx = _vfxPool.Get();
        MMSoundManagerSoundPlayEvent.Trigger(gainCurrencySound, MMSoundManager.MMSoundManagerTracks.Sfx, transform.position);
        
        StartCoroutine(vfx.PlayAnimate(amountCurrency, false,
            spawnPosition, 
            () =>
            {
                _vfxPool.Release(vfx);
                currentCurrency += amountCurrency;
                currencyUI.UpdateCurrencyUI(currentCurrency);
            }
            ));
    }

    public bool UseCurrency(int amountCurrency)
    {
        if (currentCurrency < amountCurrency)
        {
            Debug.LogWarning($"[{name} (UseCurrency)] NO ENOUGH AMOUNT CURRENCY! currentCurrency: {currentCurrency}");
            return false;
        }
        
        CurrencyVisualizer vfx = _vfxPool.Get();
        MMSoundManagerSoundPlayEvent.Trigger(useCurrencySound, MMSoundManager.MMSoundManagerTracks.Sfx, transform.position);
        
        StartCoroutine(vfx.PlayAnimate(amountCurrency, true,
            spawnPosition, 
            () =>
            {
                _vfxPool.Release(vfx);
                currentCurrency = Mathf.Max(0, currentCurrency - amountCurrency);
                currencyUI.UpdateCurrencyUI(currentCurrency);
            }
        ));
        
        return true;
    }
    
    public void TestUseCurrency(int amountCurrency)
    {
        bool money =  UseCurrency(amountCurrency);
    }
}
